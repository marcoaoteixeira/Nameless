using System.Reflection;
using Nameless.Web;

namespace Nameless.ApiVersioning.Sample.Web;

public static class EntryPoint {
    private static readonly Assembly[] SupportAssemblies = [Assembly.GetExecutingAssembly()];

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
               .AddMinimalEndpoints(SupportAssemblies)
               .AddSwagger(configure => configure.UseDefaultValuesOperationFilter())
               .AddApiVersioningDefault();

        var app = builder.Build();

        app.UseRouting()
           .UseMinimalEndpoints()
           .UseSwagger()
           .UseSwaggerUI(configure => configure.UseVersionableEndpoints(app));

        app.Run();
    }
}
