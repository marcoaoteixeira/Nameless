using Nameless.Sample.WebApp.AutoGenCode;

namespace Nameless.Sample.WebApp;

public class EntryPoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.RegisterAutoEndpoints();
        
        var app = builder.Build();

        app.MapAutoEndpoints();

        app.Run();
    }
}
