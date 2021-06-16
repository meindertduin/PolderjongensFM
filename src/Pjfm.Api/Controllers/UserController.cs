using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Common.Extensions;
using Pjfm.Application.Configuration;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Users.Queries;
using Pjfm.Domain.ValueObjects;
using pjfm.Models.Users;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("me")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.User)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var principal = HttpContext.User.GetPjfmPrincipal();
            var responseModel =  new GetUserResponseModel()
            {
                Roles = principal.Roles,
                Username = user.DisplayName,
                EmailConfirmed = user.EmailConfirmed,
                SpotifyAuthenticated = principal.SpotifyAuthenticated,
            };
            
            return Ok(responseModel);
        }

        /// <summary>
        /// Searches a user based on the provided query
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] string query)
        {
            var result = await _mediator.Send(new SearchUsersQuery()
            {
                QueryString = query,
            });

            return Ok(result.Data);
        }

        /// <summary>
        /// Gets all ApplicationUsers where the member attribute is true
        /// </summary>
        [HttpGet("members")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _mediator.Send(new GetAllPjMembersQuery());
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets all user profiles
        /// </summary>
        [HttpGet("list")]
        [Authorize(Policy = ApplicationIdentityConstants.Policies.Mod)]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            var result = await _mediator.Send(new GetAllUserProfileQuery());

            return Ok(result.Data);
        }
    }
}