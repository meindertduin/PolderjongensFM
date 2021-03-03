using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Configuration;
using Pjfm.Application.Interfaces;

namespace Pjfm.Api.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
    public class DjHub : Hub
    {
        private readonly IPlaybackInfoProvider _playbackInfoProvider;

        public DjHub(IPlaybackInfoProvider playbackInfoProvider)
        {
            _playbackInfoProvider = playbackInfoProvider;
        }
        
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("PlaybackSettings", _playbackInfoProvider.GetPlaybackSettings());
        }
    }
}