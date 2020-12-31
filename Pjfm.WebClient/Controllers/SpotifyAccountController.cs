using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Pjfm.Application.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.Services;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Domain.Interfaces;
using pjfm.Models;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/spotify/account")]
    public class SpotifyAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppDbContext _ctx;
        private readonly ISpotifyBrowserService _spotifyBrowserService;

        private const int StateStringLength = 30;

        private static ConcurrentDictionary<string, CachedAuthenticationState> _cachedStates =
            new ConcurrentDictionary<string, CachedAuthenticationState>();
        
        public SpotifyAccountController(IMediator mediator,
            ISpotifyBrowserService spotifyBrowserService,
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager,
            IAppDbContext ctx)
        {
            _mediator = mediator;
            _spotifyBrowserService = spotifyBrowserService;
            _configuration = configuration;
            _userManager = userManager;
            _ctx = ctx;
        }

        [HttpGet("authenticate")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> InitializeAuthentication()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Forbid();
            }
            
            var state = GenerateStateString();

            if (_cachedStates.ContainsKey(user.Id))
            {
                var removeResult=  _cachedStates.TryRemove(user.Id, out var cachedAuthenticationState);
                if (removeResult == false)
                {
                    return StatusCode(500);
                }
            }

            var addResult = _cachedStates.TryAdd(user.Id, new CachedAuthenticationState()
            {
                State = state,
                TimeCached = DateTime.Now,
            });

            if (addResult == false)
            {
                return StatusCode(500);
            }
            
            var authorizationUrl = "https://accounts.spotify.com/authorize" + 
                                   "?client_id=ebc49acde46148eda6128d944c067b5d" + 
                                   "&response_type=code" +
                                   $"&state={state}" + 
                                   $@"&redirect_uri={_configuration["AppUrls:ApiBaseUrl"]}/api/spotify/account/callback" + 
                                   "&scope=user-top-read user-read-private streaming user-read-playback-state playlist-read-private playlist-read-collaborative";

            return Redirect(authorizationUrl);
        }
        
        
        [HttpGet("callback")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> FinalizeAuthentication(string state, string code)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var trackedUserProfile = _ctx.ApplicationUsers.FirstOrDefault(a => a.Id == user.Id);

            if (user == null || trackedUserProfile == null)
            {
                return Forbid();
            }

            var removeResult=  _cachedStates.TryRemove(user.Id, out var cachedAuthenticationState);
            if (removeResult == false)
            {
                return Forbid();
            }

            if (state != cachedAuthenticationState.State || 
                DateTime.Now - cachedAuthenticationState.TimeCached > TimeSpan.FromSeconds(60))
            {
                return Forbid();
            }

            var result = await _mediator.Send(new AccessTokensRequestCommand()
            {
                Code = code,
                RedirectUri = _configuration["Spotify:CallbackUrl"],
            });

            if (result.Error == false)
            {
                trackedUserProfile.SpotifyAuthenticated = true;
                trackedUserProfile.SpotifyAccessToken = result.Data.AccessToken;
                trackedUserProfile.SpotifyRefreshToken = result.Data.RefreshToken;
                await _ctx.SaveChangesAsync(CancellationToken.None);

                var setTopTracksResult = await _mediator.Send(new UpdateUserTopTracksCommand()
                {
                    User = user,
                });
            
                if (setTopTracksResult.Error == false)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    if (userClaims.Any(x => x.Type == SpotifyIdentityConstants.Claims.SpStatus && x.Value == SpotifyIdentityConstants.Roles.Auth) == false)
                    {
                        await _userManager.AddClaimAsync(user,
                            new Claim(SpotifyIdentityConstants.Claims.SpStatus, SpotifyIdentityConstants.Roles.Auth));
                    }
                    
                    return Redirect(_configuration["AppUrls:ClientBaseUrl"]);
                }
            }

            return BadRequest();
        }

        private void EmptyUnUsedCachedStates()
        {
            
        }
        
        [HttpGet("me")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var topTracksResult = await _spotifyBrowserService.Me(user.Id, user.SpotifyAccessToken);

            var content = await topTracksResult.Content.ReadAsStringAsync();
            
            return Ok(content);
        }

        private string GenerateStateString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rand = new Random();

            return new String(Enumerable.Repeat(chars, StateStringLength)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}