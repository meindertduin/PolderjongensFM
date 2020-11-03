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
using pjfm.Models;
using Pjfm.WebClient.Services;

namespace pjfm.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
    public class RadioHub : Hub, IObserver<bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly IPlaybackController _playbackController;
        
        private IDisposable _unsubscriber;

        public RadioHub(UserManager<ApplicationUser> userManager, IPlaybackListenerManager playbackListenerManager, 
            IPlaybackController playbackController)
        {
            _userManager = userManager;
            _playbackListenerManager = playbackListenerManager;
            _playbackController = playbackController;

            _unsubscriber = _playbackController.SubscribeToPlayingStatus(this);
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
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

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(bool value)
        {
            if (value)
            {
                var playingTrackInfo = _playbackController.GetPlayingTrackInfo();
                
                var trackInfo = new PlayerUpdateInfoModel
                {
                    CurrentPlayingTrack = playingTrackInfo.Item1,
                    StartingTime = playingTrackInfo.Item2,
                };

                Clients.All.SendAsync("ReceivePlayingTrackInfo", trackInfo);
            }
        }
    }
}