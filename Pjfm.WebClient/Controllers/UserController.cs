using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] string query)
        {
            var result = await _mediator.Send(new SearchUsersQuery()
            {
                QueryString = query,
            });

            return Ok(result.Data);    
        }

        [HttpGet("members")]
        public async Task<IActionResult> GetAllMembers()
        {
            var result = await _mediator.Send(new GetAllPjMembersQuery());
            return Ok(result.Data);
        }
        
        [HttpGet("list")]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            var result = await _mediator.Send(new GetAllUserProfileQuery());

            return Ok(result.Data);
        }
    }
}