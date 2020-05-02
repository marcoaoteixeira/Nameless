using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nameless.WebApplication.Web {
    public class EntryPoint {
        #region Public Static Methods

        public static void Main (string[] args) {
            CreateWebHostBuilder (args).Build ().Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) {
            return WebHost
                .CreateDefaultBuilder (args)
                .UseKestrel ()
                .UseContentRoot (Directory.GetCurrentDirectory ())
                .ConfigureAppConfiguration ((ctx, config) => {
                    config.AddJsonFile ("AppSettings.json", optional : true, reloadOnChange : true);
                    config.AddJsonFile ($"AppSettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional : true);
                    config.AddEnvironmentVariables ();
                })
                .UseStartup<StartUp> ();
        }

        #endregion
    }
}