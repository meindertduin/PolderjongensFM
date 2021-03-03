using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pjfm.Application.Common.Mediatr;
using Pjfm.Application.Common.Mediatr.Wrappers;
using Pjfm.Domain.Entities;

namespace Pjfm.Application.AppContexts.Auth.Commands
{
    public class LoginCommand : IRequestWrapper<ApplicationUser>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IHandlerWrapper<LoginCommand, ApplicationUser>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        public async Task<Response<ApplicationUser>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var signingResult = await _signInManager.PasswordSignInAsync(request.EmailAddress, request.Password, true, false);
            if (signingResult.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.EmailAddress);
                return Response.Ok("login succeeded", user);
            }
            
            return Response.Fail<ApplicationUser>("password and username do not match");
        }
    }
}