using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace Pjfm.Bff
{
    public partial class Startup
    {
        private void ConfigureBff(IServiceCollection services)
        {
            {
                services.AddControllers();
                services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    });
                });

                services.Configure<ForwardedHeadersOptions>(opt =>
                {
                    opt.ForwardedHeaders = ForwardedHeaders.XForwardedProto;

                    opt.KnownNetworks.Clear();
                    opt.KnownProxies.Clear();
                });

                services.AddHttpContextAccessor();
                services.AddHttpClient();
            }
        }
    }
}