using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pjfm.Application.Auth.Querys;

namespace pjfm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var logoutResult = await _mediator.Send(new LogoutCommand()
            {
                LogoutId = logoutId,
            });

            if (logoutResult.Error)
            {
                return BadRequest();
            }

            return Redirect(logoutResult.Data);
        } 
    }
}