using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pjfm.Application.Identity;
using Pjfm.Application.MediatR;
using Pjfm.Application.MediatR.Wrappers;

namespace Pjfm.Application.Auth.Querys
{
    public class LoginCommand : IRequestWrapper<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IHandlerWrapper<LoginCommand, string>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginCommandHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public async Task<Response<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var signingResult = await _signInManager.PasswordSignInAsync(request.Username, request.Password, true, false);
            if (signingResult.Succeeded)
            {
                return Response.Ok<string>("login succeeded", "");
            }
            
            return Response.Fail<string>("password and username do not match");
        }
    }
}