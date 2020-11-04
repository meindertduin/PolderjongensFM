using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pjfm.Domain.Common;

namespace Pjfm.Infrastructure.Service
{
    public interface ISpotifyTrackSerializer
    {
        List<object> ConvertObject(string jsonString);
    }
}