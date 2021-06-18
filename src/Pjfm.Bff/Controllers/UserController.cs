﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Pjfm.Bff.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("login")]
        public IActionResult Login()
        {
            var loginUrl = $"{_configuration.GetValue<string>("BackendUrl")}/account/login";
            return Redirect(loginUrl);
        }
        
        [Route("login")]
        public IActionResult Register()
        {
            var loginUrl = $"{_configuration.GetValue<string>("BackendUrl")}/account/register";
            return Redirect(loginUrl);
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            return SignOut("cookies", "oidc");
        }
    }
}