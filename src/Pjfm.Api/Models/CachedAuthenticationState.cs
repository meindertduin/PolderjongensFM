using System;

namespace Pjfm.Api.Models
{
    public class CachedAuthenticationState
    {
        public string State { get; set; }
        public DateTime TimeCached { get; set; }
    }
}