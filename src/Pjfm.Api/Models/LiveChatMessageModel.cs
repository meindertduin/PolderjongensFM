using System;

namespace pjfm.Models
{
    public class LiveChatMessageModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime TimeSend { get; set; }
    }
}