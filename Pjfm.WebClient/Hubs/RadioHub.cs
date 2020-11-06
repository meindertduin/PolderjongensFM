﻿using System;
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
using pjfm.Models;
using Pjfm.WebClient.Services;

namespace pjfm.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
    public class RadioHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly IPlaybackEventTransmitter _playbackEventTransmitter;
        private readonly IPlaybackController _playbackController;

        public RadioHub(UserManager<ApplicationUser> userManager, IPlaybackListenerManager playbackListenerManager, 
            IPlaybackEventTransmitter playbackEventTransmitter, IPlaybackController playbackController)
        {
            _userManager = userManager;
            _playbackListenerManager = playbackListenerManager;
            _playbackEventTransmitter = playbackEventTransmitter;
            _playbackController = playbackController;
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            var infoModelFactory = new PlaybackInfoFactory(_playbackController);
            var userInfo = infoModelFactory.CreateUserInfoModel();

            Clients.Caller.SendAsync("ReceivePlayingTrackInfo", userInfo);

            await _playbackListenerManager.AddListener(user);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            _playbackListenerManager.RemoveListener(user.Id);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}