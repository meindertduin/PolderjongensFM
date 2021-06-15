using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Users.Queries;
using Pjfm.Domain.Common;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly PjfmPrincipal _principal;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
            // _principal = new PjfmPrincipal(HttpContext.User);
        }

        [HttpGet("/me")]
        public IActionResult GetCurrentUser()
        {
            // var roles = _principal.Roles;

            return Ok();
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