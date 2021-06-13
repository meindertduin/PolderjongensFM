using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Pjfm.Bff
{
    public partial class Startup
    {
        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                };
            });
        }
    }
}