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

namespace pjfm.Hubs
{
    [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
    public class RadioHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        private static readonly ConcurrentDictionary<string, ApplicationUser> _connectedUsers 
            = new ConcurrentDictionary<string, ApplicationUser>();

        public RadioHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            _connectedUsers[user.Id] = user;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            _connectedUsers.TryRemove(user.Id, out user);
            await base.OnDisconnectedAsync(exception);
        }
    }
}