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
        private static ApplicationUser _connectedUser;
        
        private readonly object _djConnectionLock = new object();


        public DjHub(UserManager<ApplicationUser> userManager, IPlaybackController playbackController)
        {
            _userManager = userManager;
            _playbackController = playbackController;
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            TryConnectAsDj(user);

            var playingTrackInfo = _playbackController.GetPlayingTrackInfo();
            var fillerQueuedTracks = _playbackController.GetFillerQueueTracks();
            var queuedPriorityTracks = _playbackController.GetPriorityQueueTracks();
                
            var djInfo = new DjPlaybackInfoModel
            {
                CurrentPlayingTrack = playingTrackInfo.Item1,
                StartingTime = playingTrackInfo.Item2,
                FillerQueuedTracks = fillerQueuedTracks,
                PriorityQueuedTracks = queuedPriorityTracks
            };

            Clients.Caller.SendAsync("ReceiveDjPlaybackInfo", djInfo);
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

    }
}