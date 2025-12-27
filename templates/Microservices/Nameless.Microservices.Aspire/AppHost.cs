using Projects;

namespace Nameless.Microservices.Aspire;

public sealed class AppHost {
    public static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        builder
            .AddProject<Nameless_Microservices_App>("MainApp")
            .WithHttpHealthCheck("/health");

        builder.Build().Run();
    }
}