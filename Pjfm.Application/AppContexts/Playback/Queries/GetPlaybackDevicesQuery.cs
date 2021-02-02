using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;
using Pjfm.Domain.Interfaces;

namespace Pjfm.Application.Common.Dto.Queries
{
    public class GetPlaybackDevicesQuery : IRequestWrapper<List<PlaybackDevice>>
    {
        public string UserId { get; set; }
    }

    public class GetPlaybackDevicesQueryHandler : IHandlerWrapper<GetPlaybackDevicesQuery, List<PlaybackDevice>>
    {
        private readonly ISpotifyPlayerService _spotifyPlayerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetPlaybackDevicesQueryHandler(ISpotifyPlayerService spotifyPlayerService, UserManager<ApplicationUser> userManager)
        {
            _spotifyPlayerService = spotifyPlayerService;
            _userManager = userManager;
        }
        
        public async Task<Response<List<PlaybackDevice>>> Handle(GetPlaybackDevicesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user != null)
            {
                var responseMessage = await _spotifyPlayerService.GetDevices(user.Id, user.SpotifyAccessToken);
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                var rootObject = JsonConvert.DeserializeObject<RootObject>(jsonData);

                return Response.Ok("query devices was succesfull", rootObject.Devices);

            }
            
            return Response.Fail<List<PlaybackDevice>>("something went wrong while querying devices");
        }
    }

    public class RootObject
    {
        public List<PlaybackDevice> Devices { get; set; }
    }
}