using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;

namespace pjfm.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
    public class RadioHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpotifyPlaybackManager _spotifyPlaybackManager;
        public RadioHub(UserManager<ApplicationUser> userManager, ISpotifyPlaybackManager spotifyPlaybackManager)
        {
            _userManager = userManager;
            _spotifyPlaybackManager = spotifyPlaybackManager;
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            _spotifyPlaybackManager.AddListener(user);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            _spotifyPlaybackManager.RemoveListener(user.Id);
            await base.OnDisconnectedAsync(exception);
        }
    }
}