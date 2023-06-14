
using Autofac.Extensions.DependencyInjection;

namespace Nameless.MicroWeb {
    public static class EntryPoint {

        #region Public Static Methods

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(builder => {
                    builder
                        .ConfigureAppConfiguration((ctx, config) => {
                            config.AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);
                            config.AddJsonFile($"AppSettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                            config.AddEnvironmentVariables();
                            config.AddUserSecrets<StartUp>();
                        })
                        .ConfigureLogging((webHostBuilderContext, loggingBuilder) => {
                            loggingBuilder.AddConfiguration(webHostBuilderContext.Configuration.GetSection("Logging"));
                            loggingBuilder.AddConsole();
                            loggingBuilder.AddDebug();
                        })
                        .UseStartup<StartUp>();
                });

        #endregion
    }

    //public class EntryPoint {
    //    public static void Main(string[] args) {
    //        var builder = WebApplication.CreateBuilder(args);

    //        // Add services to the container.

    //        builder.Services.AddControllers();
    //        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //        builder.Services.AddEndpointsApiExplorer();
    //        builder.Services.AddSwaggerGen();

    //        var app = builder.Build();

    //        // Configure the HTTP request pipeline.
    //        if (app.Environment.IsDevelopment()) {
    //            app.UseSwagger();
    //            app.UseSwaggerUI();
    //        }

    //        app.UseHttpsRedirection();

    //        app.UseAuthorization();


    //        app.MapControllers();

    //        app.Run();
    //    }
    //}
}