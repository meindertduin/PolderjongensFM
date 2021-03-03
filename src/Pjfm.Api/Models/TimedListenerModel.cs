﻿using System;
using System.Threading;

namespace Pjfm.Api.Models
{
    public class TimedListenerModel
    {
        public string ConnectionId { get; set; }
        public CancellationTokenSource TimedListenerCancellationTokenSource { get; set; }
        public DateTime TimeAdded { get; set; }
        public int SubscribeTimeMinutes { get; set; }
    }
}