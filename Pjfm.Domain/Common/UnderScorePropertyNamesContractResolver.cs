using Newtonsoft.Json.Serialization;

namespace Pjfm.Domain.Common
{
    public class UnderScorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderScorePropertyNamesContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}