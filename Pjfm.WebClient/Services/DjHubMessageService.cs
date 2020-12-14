﻿using Microsoft.AspNetCore.SignalR;
using pjfm.Hubs;
using pjfm.Models;

namespace pjfm.Services
{
    public class DjHubMessageService : IDjHubMessageService
    {
        private readonly IHubContext<DjHub> _hubContext;

        public DjHubMessageService(IHubContext<DjHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SendMessageToClient(HubServerMessage hubServerMessage)
        {
            _hubContext.Clients.All.SendAsync("ServerMessage", hubServerMessage);
        }
    }
}