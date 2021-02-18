using Microsoft.AspNetCore.SignalR;
using Moq;
using Pjfm.Application.Common.Dto;
using Pjfm.Application.Identity;
using Pjfm.Domain.Interfaces;
using pjfm.Hubs;
using Pjfm.WebClient.Services;
using Xunit;

namespace Pjfm.WebClient.UnitTests.ServicesTests
{
    public class PlaybackListenerManagerTest
    {
        private PlaybackListenerManager _playbackListenerManager;

        public PlaybackListenerManagerTest()
        {
            _playbackListenerManager = new PlaybackListenerManager(new Mock<ISpotifyPlaybackManager>().Object,
                new Mock<IPlaybackController>().Object, new Mock<IHubContext<RadioHub>>().Object);
        }
        
        [Fact]
        public void RemoveListener_AfterAddingValidListener_ReturnsNotNull()
        {
                        
            var fakeUserId = "0";
            var fakeUser = new ApplicationUser("fake@mail.com"){ Id = fakeUserId};
            var fakeDevice = new PlaybackDevice() {Id = "1234"};
            
            
            _playbackListenerManager.AddListener(fakeUser, fakeDevice);
            var removedUser = _playbackListenerManager.RemoveListener(fakeUserId);
            Assert.NotNull(removedUser);
        }

        [Fact]
        public void TrySetTimedListener_WithValidTimeAndId_ReturnsTrue()
        {
            var fakeUserId = "1";
            var minutes = 5;
            var fakeConnectionId = "10";

            var setTimedListenerResult = _playbackListenerManager.TrySetTimedListener(fakeUserId, minutes, fakeConnectionId);
            _playbackListenerManager.TryRemoveTimedListener(fakeUserId);
            
            Assert.True(setTimedListenerResult);
        }

        [Fact]
        public void TryRemoveTimedListener_AfterAddingTimedListener_ReturnsTrue()
        {
            var fakeUserId = "2";
            var minutes = 5;
            var fakeConnectionId = "20";
            
            _playbackListenerManager.TrySetTimedListener(fakeUserId, minutes, fakeConnectionId);
            var removeResult = _playbackListenerManager.TryRemoveTimedListener(fakeUserId);
            
            Assert.True(removeResult);
        }

        [Fact]
        public void TryRemoveTimedListener_WithoutAddingUser_ReturnsFalse()
        {
            var fakeUserId = "3";
            var removeResult = _playbackListenerManager.TryRemoveTimedListener(fakeUserId);
            
            Assert.False(removeResult);
        }

        [Fact]
        public void TryRemoveTimedListener_TryRemoveUserTwice_ReturnsFalseSecondTime()
        {
            var fakeUserId = "4";
            var minutes = 5;
            var fakeConnectionId = "40";
            
            _playbackListenerManager.TrySetTimedListener(fakeUserId, minutes, fakeConnectionId);
            _playbackListenerManager.TryRemoveTimedListener(fakeUserId);
            var removeResult = _playbackListenerManager.TryRemoveTimedListener(fakeUserId);
            
            Assert.False(removeResult);
        }

        [Fact]
        public void GetUserSubscribeTime_AfterAddingUserId5Minutes_ReturnsFive()
        {
            var fakeUserId = "5";
            var minutes = 5;
            var fakeConnectionId = "50";
            _playbackListenerManager.TrySetTimedListener(fakeUserId, minutes, fakeConnectionId);
            var subscribeTime = _playbackListenerManager.GetUserSubscribeTime(fakeUserId);
            
            Assert.Equal(minutes, subscribeTime);
        }

        [Fact]
        public void GetUserSubscribeTime_WithNonAddedUserId_ReturnsNull()
        {
            var fakeUserId = "6";
            var subscribeTime = _playbackListenerManager.GetUserSubscribeTime(fakeUserId);
            
            Assert.Null(subscribeTime);
        }
    }
}