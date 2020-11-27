﻿using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;
using Pjfm.Application.Spotify.Commands;
using Pjfm.Application.Test.Queries;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        
        [HttpGet("user")] 
        public string GetUserMessage()
        {
            var claims = HttpContext.User;

            return "user";
        }

        [HttpGet("mod")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public string GetModMessage()
        {
            return "mod";
        }

        [HttpGet("toptracks")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetUserTopTracks()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var queryResult = await _mediator.Send(new GetUserTopTracksQuery
            {
                UserID = user.Id,
            });

            if (queryResult.Error)
            {
                return BadRequest();
            }
            
            return Ok(queryResult.Data);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var user = HttpContext.User;
            var userProfile = await _userManager.GetUserAsync(user);
            
            var refreshResult = await _mediator.Send(new AccessTokenRefreshCommand()
            {
                UserId = userProfile.Id,
            });

            return Ok(refreshResult.Data);
        }
    }
}