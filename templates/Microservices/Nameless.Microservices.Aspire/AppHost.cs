namespace Nameless.Microservices.Aspire;

public sealed class AppHost {
    public static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.Build().Run();
    }
}