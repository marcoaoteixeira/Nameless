using Nameless.Identity.Aspire.AppHost.Configs;

namespace Nameless.Identity.Aspire.AppHost;

public static class EntryPoint {
    public static void Main(string[] args) {
        DistributedApplication.CreateBuilder(args)
                              .RegisterMainApplication()
                              .Build()
                              .Run();
    }
}