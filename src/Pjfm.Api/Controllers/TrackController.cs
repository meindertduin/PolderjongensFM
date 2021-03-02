using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pjfm.Application.Identity;
using Pjfm.Application.Mappings;
using Pjfm.Application.Services;
using Pjfm.Domain.Common;
using Pjfm.Infrastructure.Service;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("track")]
    public class TrackController : ControllerBase
    {
        private readonly ISpotifyBrowserService _spotifyBrowserService;

        public TrackController(ISpotifyBrowserService spotifyBrowserService)
        {
            _spotifyBrowserService = spotifyBrowserService;
        }
       
        /// <summary>
        /// Gets multiple trackDto's of the provided trackIds
        /// </summary>
        /// <param name="trackIds">String array value of the trackDto id</param>
        [HttpGet("multiple")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetTrackInfo([FromQuery] string[] trackIds)
        {
            var response = await _spotifyBrowserService.ServerGetMultipleTracks(trackIds);
            
            if (response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync();
                var jObject = JsonConvert.DeserializeObject<dynamic>(rawJson, new JsonSerializerSettings()
                {
                    ContractResolver = new UnderScorePropertyNamesContractResolver(),
                });
                var tracksMapper = new TrackDtoMapper();
                return Ok(tracksMapper.MapObjects(jObject));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return BadRequest();
            
            return StatusCode(500);
        }
    }
}