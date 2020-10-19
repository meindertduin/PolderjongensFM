using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;

namespace Pjfm.Application.Auth.Querys
{
    public class LogoutCommand : IRequestWrapper<string>
    {
        public string LogoutId { get; set; }
    }

    public class LogoutCommandHandler : IHandlerWrapper<LogoutCommand, string>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public LogoutCommandHandler([FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _interactionService = interactionService;
        }
        
        public async Task<Response<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();

            var logoutContext = await _interactionService.GetLogoutContextAsync(request.LogoutId);

            if (string.IsNullOrEmpty(logoutContext.PostLogoutRedirectUri))
            {
                return Response.Fail<string>("no post logout redirect url could be retrieved");
            }

            return Response.Ok("logout succeeded", logoutContext.PostLogoutRedirectUri);
        }
    }
}