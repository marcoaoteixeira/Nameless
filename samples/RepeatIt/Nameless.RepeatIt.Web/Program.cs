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
                    var logger = loggerFactory.CreateLogger<PrintRecurringTask>();

                    return new PrintRecurringTask(TimeSpan.FromSeconds(2), true, logger);
                });

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}