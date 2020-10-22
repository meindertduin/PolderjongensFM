using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Application.Identity;
using pjfm.Models;

namespace pjfm.Hubs
{
    public class LiveChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LiveChatHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task SendMessage(string message)
        {
            var context = Context.GetHttpContext();
            var user = await _userManager.GetUserAsync(context.User);
            
            await Clients.All.SendAsync("ReceiveMessage", new LiveChatMessageModel
            {
                UserName = user.UserName,
                Message = message,
                TimeSend = DateTime.Now,
            });
        }
    }
}