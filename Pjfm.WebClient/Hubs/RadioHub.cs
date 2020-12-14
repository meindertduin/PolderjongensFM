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
            var userPlayerBackInfo = infoModelFactory.CreateUserInfoModel();

            await Clients.Caller.SendAsync("ReceivePlayingTrackInfo", userPlayerBackInfo);

            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);

            if (user != null)
            {
                await Clients.Caller.SendAsync("IsConnected", _playbackListenerManager.IsUserTimedListener(user.Id));
            }
            await Clients.Caller.SendAsync("ReceivePlayingStatus", _playbackController.GetPlaybackSettings().IsPlaying);
            
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
                await Clients.Caller.SendAsync("ISConnected", true);
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