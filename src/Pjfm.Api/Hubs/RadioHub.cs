using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Common.Dto.Queries;
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
        private readonly IMediator _mediator;


        public static int ListenersCount = 0;
        
        public RadioHub(UserManager<ApplicationUser> userManager, IPlaybackListenerManager playbackListenerManager, 
            IPlaybackController playbackController, ISpotifyPlayerService spotifyPlayerService, IMediator mediator)
        {
            _userManager = userManager;
            _playbackListenerManager = playbackListenerManager;
            _playbackController = playbackController;
            _spotifyPlayerService = spotifyPlayerService;
            _mediator = mediator;
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
           
            await Clients.All.SendAsync("ListenersCountUpdate", ListenersCount);
            
            await base.OnConnectedAsync();
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task ConnectWithPlayer(int minutes, PlaybackDevice device)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            // set user as timed user if spotify authenticated and send some playback status info
            if (user.SpotifyAuthenticated)
            {
                if (device == null)
                {
                    await _playbackListenerManager.AddListener(user, await GetPlaybackDevice(user.Id));
                }
                else
                {
                    await _playbackListenerManager.AddListener(user, device);
                }

                if (minutes != 0)
                {
                    var result = _playbackListenerManager.TrySetTimedListener(user.Id, minutes, Context.ConnectionId);
                    if (result)
                    {
                        Interlocked.Increment(ref ListenersCount);
                        await Clients.Caller.SendAsync("SubscribeTime", _playbackListenerManager.GetUserSubscribeTime(user.Id));
                        await Clients.All.SendAsync("ListenersCountUpdate", ListenersCount);
                        await Clients.Caller.SendAsync("IsConnected", true);    
                    }
                }
            }
        }

        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task DisconnectWithPlayer()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            _playbackListenerManager.TryRemoveTimedListener(user.Id);
            Interlocked.Decrement(ref ListenersCount);
            await Clients.Caller.SendAsync("IsConnected", false);
            await Clients.All.SendAsync("ListenersCountUpdate", ListenersCount);
            await _spotifyPlayerService.PausePlayer(user.Id, user.SpotifyAccessToken, String.Empty);
        }

        private async Task<PlaybackDevice> GetPlaybackDevice(string userId)
        {
            var result = await _mediator.Send(new GetPlaybackDevicesQuery()
            {
                UserId = userId,
            });

            if (result.Data.Count > 0)
            {
                return result.Data[0];
            }

            return null;
        }
    }
}