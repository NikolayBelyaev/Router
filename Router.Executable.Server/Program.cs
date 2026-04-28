using System.Net;
using Microsoft.AspNetCore;

namespace Router.Executable.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                })
                .UseStartup<Startup>()
                .UseKestrel(options => { options.Listen(IPAddress.Any, 5000); });
        }
    }
}