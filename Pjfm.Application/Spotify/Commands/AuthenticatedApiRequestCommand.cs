using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Pjfm.Application.Spotify.Commands
{
    public class AuthenticatedApiRequestCommand : IRequestWrapper<HttpResponseMessage>
    {
        public HttpRequestMessage RequestMessage { get; set; }
        public string UserId { get; set; }
    }

    public class
        AuthenticatedApiRequestCommandHandler : IHandlerWrapper<AuthenticatedApiRequestCommand, HttpResponseMessage>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAppDbContext _appDbContext;
        private readonly IMediator _mediator;

        public AuthenticatedApiRequestCommandHandler(IHttpClientFactory clientFactory, IAppDbContext appDbContext, 
            IMediator mediator)
        {
            _clientFactory = clientFactory;
            _appDbContext = appDbContext;
            _mediator = mediator;
        }
        
        public async Task<Response<HttpResponseMessage>> Handle(AuthenticatedApiRequestCommand request, CancellationToken cancellationToken)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var result = await client.SendAsync(request.RequestMessage, cancellationToken);
                if (result.IsSuccessStatusCode == false)
                {
                    var refreshResult = await _mediator.Send(new AccessTokenRefreshCommand()
                    {
                        UserId = request.UserId,
                    });

                    request.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Data);
                    
                    return await _mediator.Send(new AuthenticatedApiRequestCommand()
                    {
                        RequestMessage = request.RequestMessage,
                        UserId = request.UserId,
                    });
                }
                return Response.Ok("api request succeeded", result);
            }
        }
    }
}