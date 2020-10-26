using System;

namespace Pjfm.Domain.Entities
{
    public class LiveChatMessage
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime TimeSend { get; set; }
    }
}