using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using Pjfm.Application.Interfaces;
using pjfm.Models;
using Pjfm.WebClient.Services;

namespace pjfm.Hubs
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