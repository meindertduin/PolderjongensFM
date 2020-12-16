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
            await Clients.All.SendAsync("ReceivePlaybackInfo", userInfo);

            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            if (user != null)
            {
                await Clients.Caller.SendAsync("IsConnected", _playbackListenerManager.IsUserTimedListener(user.Id));
                await Clients.Caller.SendAsync("SubscribeTime", _playbackListenerManager.GetUserSubscribeTime(user.Id));
            }
            
            await Clients.Caller.SendAsync("ReceivePlayingStatus", _playbackController.GetPlaybackSettings().IsPlaying);

            var playbackSettings = _playbackController.GetPlaybackSettings();
            
            await Clients.All.SendAsync("PlaybackSettings", new UserPlaybackSettingsModel()
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
            
            await _playbackListenerManager.AddListener(user);

            if (minutes != 0)
            {
                _playbackListenerManager.TrySetTimedListener(user.Id, minutes);
                await Clients.Caller.SendAsync("SubscribeTime", _playbackListenerManager.GetUserSubscribeTime(user.Id));
                await Clients.Caller.SendAsync("IsConnected", true);
            }
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task DisconnectWithPlayer()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            _playbackListenerManager.TryRemoveTimedListener(user.Id);
            await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken, String.Empty);
        }
        
    }
}