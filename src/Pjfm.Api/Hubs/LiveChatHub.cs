using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pjfm.Api.Models;
using Pjfm.Application.Common.Interfaces;
using Pjfm.Application.Configuration;
using Pjfm.Domain.Entities;

namespace Pjfm.Api.Hubs
{
    public class LiveChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppDbContext _ctx;

        public LiveChatHub(UserManager<ApplicationUser> userManager, IAppDbContext ctx)
        {
            _userManager = userManager;
            _ctx = ctx;
        }
        
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task SendMessage(string message)
        {
            if (message.Length <= 200)
            {
                var context = Context.GetHttpContext();
                var user = await _userManager.GetUserAsync(context.User);

                var liveChatMessage = new LiveChatMessage
                {
                    UserName = user.UserName,
                    Message = message,
                    TimeSend = DateTime.Now,
                };
            
                await Clients.All.SendAsync("ReceiveMessage", liveChatMessage);
                await _ctx.LiveChatMessages.AddAsync(liveChatMessage);
                await _ctx.SaveChangesAsync(CancellationToken.None);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var lastDayChatMessages = _ctx.LiveChatMessages
                .OrderByDescending(l => l.TimeSend)
                .Select(l => new LiveChatMessageModel
                {
                    UserName = l.UserName,
                    Message = l.Message,
                    TimeSend = l.TimeSend,
                })
                .Take(10)
                .ToArray();
            
            await Clients.Caller.SendAsync("LoadMessages", lastDayChatMessages.Reverse());
        }
    }
}