using System;

namespace pjfm.Models
{
    public class CachedAuthenticationState
    {
        public string State { get; set; }
        public DateTime TimeCached { get; set; }
    }
}