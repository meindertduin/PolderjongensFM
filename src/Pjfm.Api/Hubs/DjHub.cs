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
        private readonly IPlaybackController _playbackController;

        public DjHub(IPlaybackController playbackController)
        {
            _playbackController = playbackController;
        }
        
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("PlaybackSettings", _playbackController.GetPlaybackSettings());
        }
    }
}