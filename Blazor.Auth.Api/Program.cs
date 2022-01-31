using Blasor.Data;
using Blazor.Auth.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace Blazor.Auth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .CreateDatabase<IdentityContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(80, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                    });
                    webBuilder.UseStartup<Startup>();

                });
    }
}
