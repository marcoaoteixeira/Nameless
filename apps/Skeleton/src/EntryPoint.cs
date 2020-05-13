using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nameless.Skeleton.Web {
    public sealed class EntryPoint {
        #region Public Static Methods

        public static void Main (string[] args) {
            CreateHostBuilder (args).Build ().Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) {
            return Host
                .CreateDefaultBuilder (args)
                .UseServiceProviderFactory (new AutofacServiceProviderFactory ())
                .ConfigureWebHostDefaults (builder => {
                    builder
                        .UseKestrel ()
                        .UseIISIntegration ()
                        .UseContentRoot (Directory.GetCurrentDirectory ())
                        .ConfigureAppConfiguration ((ctx, config) => {
                            config.AddJsonFile ("AppSettings.json", optional : true, reloadOnChange : true);
                            config.AddJsonFile ($"AppSettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional : true);
                            config.AddEnvironmentVariables ();
                        })
                        .UseStartup<StartUp> ();
                });
        }

        #endregion
    }
}