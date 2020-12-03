using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
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
        private readonly ISpotifyPlayerService _spotifyPlayerService;

        public RadioHub(UserManager<ApplicationUser> userManager, IPlaybackListenerManager playbackListenerManager, 
            IPlaybackEventTransmitter playbackEventTransmitter, IPlaybackController playbackController, ISpotifyPlayerService spotifyPlayerService)
        {
            _userManager = userManager;
            _playbackListenerManager = playbackListenerManager;
            _playbackEventTransmitter = playbackEventTransmitter;
            _playbackController = playbackController;
            _spotifyPlayerService = spotifyPlayerService;
        }

        public override async Task OnConnectedAsync()
        {
            var infoModelFactory = new PlaybackInfoFactory(_playbackController);
            var userInfo = infoModelFactory.CreateUserInfoModel();

            await Clients.Caller.SendAsync("ReceivePlayingTrackInfo", userInfo);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await DisconnectWithPlayer();
            await base.OnDisconnectedAsync(exception);
        }
        
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task ConnectWithPlayer(int minutes)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            await _playbackListenerManager.AddListener(user);

            if (minutes != 0)
            {
                _playbackListenerManager.TrySetTimedListener(user.Id, minutes);
            }
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task DisconnectWithPlayer()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            if (_playbackListenerManager.IsUserTimedListener(user.Id) == false)
            {
                _playbackListenerManager.RemoveListener(user.Id);
                await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken, String.Empty);
            }
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task CancelTimedListenSession()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            _playbackListenerManager.TryRemoveTimedListener(user.Id);
        }
    }
}