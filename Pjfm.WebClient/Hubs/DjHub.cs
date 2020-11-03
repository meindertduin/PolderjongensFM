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
    public class DjHub : Hub, IObserver<bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaybackController _playbackController;
        private readonly IHubContext<DjHub> _hubContext;
        private static ApplicationUser _connectedUser;
        
        private readonly object _djConnectionLock = new object();
        private IDisposable _unsubscriber;


        public DjHub(UserManager<ApplicationUser> userManager, IPlaybackController playbackController, IHubContext<DjHub> hubContext)
        {
            _userManager = userManager;
            _playbackController = playbackController;
            _hubContext = hubContext;

            _unsubscriber = _playbackController.SubscribeToPlayingStatus(this);
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            TryConnectAsDj(user);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            if (_connectedUser.Id == user.Id)
            {
                _connectedUser = null;
            }
        }

        private void TryConnectAsDj(ApplicationUser user)
        {
            lock(_djConnectionLock)
            {
                if (_connectedUser == null)
                {
                    _connectedUser = user;
                }
                else
                {
                    Context.GetHttpContext().Abort();
                }
            }
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
                var fillerQueuedTracks = _playbackController.GetFillerQueueTracks();
                var queuedPriorityTracks = _playbackController.GetPriorityQueueTracks();
                
                var trackInfo = new DjPlaybackInfoModel
                {
                    CurrentPlayingTrack = playingTrackInfo.Item1,
                    StartingTime = playingTrackInfo.Item2,
                    FillerQueuedTracks = fillerQueuedTracks,
                    PriorityQueuedTracks = queuedPriorityTracks
                };
                
                _hubContext.Clients.All.SendAsync("ReceiveDjPlaybackInfo", trackInfo);
            }
        }
    }
}