using Nameless.Sample.Chores.Configs;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.Chores;

public static class EntryPoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
               .AddAuthorization()
               .RegisterApplicationServices()
               .ConfigureMinimalEndpoint(configure => {
                   configure.Assemblies = [typeof(EntryPoint).Assembly];
               });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseRouting()
           .UseAuthorization()
           .UseMinimalEndpoints();

        app.Run();
    }
}
