using Nameless.Microservices.Web.Configs;

namespace Nameless.Microservices.Web;

public static class EntryPoint {
    public static void Main(string[] args) {
        WebApplication.CreateBuilder(args)

                      .RegisterMinimalEndpoints()

                      .Build()

                      .ResolveMinimalEndpoints()

                      .Run();
    }
}
