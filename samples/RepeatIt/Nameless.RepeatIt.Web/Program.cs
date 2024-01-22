using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Infrastructure;

namespace Nameless.RepeatIt.Web {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder
                .Services
                .AddLogging();

            builder
                .Services
                .AddHostedService(factory => {
                    var loggerFactory = factory.GetRequiredService<ILoggerFactory>();

                    //var logger = loggerFactory.CreateLogger<RepeatingBackgroundTask>();
                    //return new RepeatingBackgroundTask(logger);

                    var logger = loggerFactory.CreateLogger<PrintRecurringTask>();
                    return new PrintRecurringTask(TimeSpan.FromSeconds(2), logger);
                });

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            app.MapPost("/speedup", ([FromServices] IServiceProvider provider) => {
                var recurring = provider
                    .GetServices<IHostedService>()
                    .Where(hostedService => typeof(RecurringTaskHostedService).IsAssignableFrom(hostedService.GetType()))
                    .FirstOrDefault();

                if (recurring is RecurringTaskHostedService recurringTaskHostedService) {
                    recurringTaskHostedService.SetInterval(TimeSpan.FromSeconds(0.5));
                }
            });

            app.Run();
        }
    }
}