using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using pjfm.Models;
using Pjfm.WebClient.Services;

namespace pjfm.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
    public class DjHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaybackController _playbackController;

        public DjHub(UserManager<ApplicationUser> userManager, IPlaybackController playbackController)
        {
            _userManager = userManager;
            _playbackController = playbackController;
        }
        
        public override async Task OnConnectedAsync()
        {
            var infoModelFactory = new PlaybackInfoFactory(_playbackController);
            var djInfo = infoModelFactory.CreateDjInfoModel();

            await Clients.Caller.SendAsync("ReceiveDjPlaybackInfo", djInfo);
        }
    }
}