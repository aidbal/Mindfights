using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Mindfights.Web.Host.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:21021")
                .Build();
        }
    }
}
