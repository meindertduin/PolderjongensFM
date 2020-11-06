﻿using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using pjfm.Hubs;
using pjfm.Models;

namespace Pjfm.WebClient.Services
{
    public class PlaybackEventTransmitter : IPlaybackEventTransmitter, IObserver<bool>
    {
        private readonly IPlaybackController _playbackController;
        private readonly IServiceProvider _serviceProvider;
        private IDisposable _unsubscriber;

        public PlaybackEventTransmitter(IPlaybackController playbackController, IServiceProvider serviceProvider)
        {
            _playbackController = playbackController;
            _serviceProvider = serviceProvider;
            _unsubscriber = _playbackController.SubscribeToPlayingStatus(this);
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var radioHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<RadioHub>>();
                    var djHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<DjHub>>();
                    
                    var infoModelFactory = new PlaybackInfoFactory(_playbackController);
                    var userInfo = infoModelFactory.CreateUserInfoModel();
                    var djInfo = infoModelFactory.CreateUserInfoModel();
                    

                    radioHubContext.Clients.All.SendAsync("ReceivePlayingTrackInfo", userInfo);
                
                    djHubContext.Clients.All.SendAsync("ReceiveDjPlaybackInfo", djInfo);
                }
            }
        }
    }

    public interface IPlaybackEventTransmitter
    {
        
    }
}