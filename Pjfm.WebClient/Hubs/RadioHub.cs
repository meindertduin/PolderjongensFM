using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
using pjfm.Models;
using Pjfm.WebClient.Services;

namespace pjfm.Hubs
{
    public class RadioHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPlaybackListenerManager _playbackListenerManager;
        private readonly IPlaybackController _playbackController;
        private readonly ISpotifyPlayerService _spotifyPlayerService;

        private static int _listenerCount = 0;

        public RadioHub(UserManager<ApplicationUser> userManager, IPlaybackListenerManager playbackListenerManager, 
            IPlaybackController playbackController, ISpotifyPlayerService spotifyPlayerService)
        {
            _userManager = userManager;
            _playbackListenerManager = playbackListenerManager;
            _playbackController = playbackController;
            _spotifyPlayerService = spotifyPlayerService;
        }
        
        public override async Task OnConnectedAsync()
        {
            // turn playback on if its turned off
            if (_playbackController.IsPlaying() == false)
            {
                _playbackController.TurnOn(PlaybackControllerCommands.TrackPlayerOnOff);
            }
            
            // send caller updated playback info
            var infoModelFactory = new PlaybackInfoFactory(_playbackController);
            var userInfo = infoModelFactory.CreateUserInfoModel();
            await Clients.Caller.SendAsync("ReceivePlaybackInfo", userInfo);

            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            // sends the caller the subscribe time and connected state, only if user is authenticated
            if (user != null)
            {
                await Clients.Caller.SendAsync("IsConnected", _playbackListenerManager.IsUserTimedListener(user.Id));
                await Clients.Caller.SendAsync("SubscribeTime", _playbackListenerManager.GetUserSubscribeTime(user.Id));
            }
            
            await Clients.Caller.SendAsync("ReceivePlayingStatus", _playbackController.GetPlaybackSettings().IsPlaying);
            
            var playbackSettings = _playbackController.GetPlaybackSettings();
            
            await Clients.Caller.SendAsync("PlaybackSettings", new UserPlaybackSettingsModel()
            {
                PlaybackState = playbackSettings.PlaybackState,
                IsPlaying = playbackSettings.IsPlaying,
                MaxRequestsPerUser = playbackSettings.MaxRequestsPerUser,
            });
            
            await base.OnConnectedAsync();
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task ConnectWithPlayer(int minutes)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            // set user as timed user if spotify authenticated and send some playback status info
            if (user.SpotifyAuthenticated)
            {
                await _playbackListenerManager.AddListener(user);

                if (minutes != 0)
                {
                    _playbackListenerManager.TrySetTimedListener(user.Id, minutes, Context.ConnectionId);
                    await Clients.Caller.SendAsync("SubscribeTime", _playbackListenerManager.GetUserSubscribeTime(user.Id));
                    await Clients.Caller.SendAsync("IsConnected", true);
                }
            }
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task DisconnectWithPlayer()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            _playbackListenerManager.TryRemoveTimedListener(user.Id);
            await Clients.Caller.SendAsync("IsConnected", false);
            await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken, String.Empty);
        }
        
    }
}