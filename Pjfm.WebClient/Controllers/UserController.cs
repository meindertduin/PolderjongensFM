using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR.Users.Queries;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
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