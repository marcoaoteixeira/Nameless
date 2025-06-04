using Nameless.Microservices.Aspire.AppHost.Configs;

namespace Nameless.Microservices.Aspire.AppHost;

public static class EntryPoint {
    public static void Main(string[] args) {
        DistributedApplication.CreateBuilder(args)
                              .RegisterMainApplication()
                              .Build()
                              .Run();
    }
}