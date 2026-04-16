using Nameless.Microservice.AppHost.Infrastructure;
using Projects;

namespace Nameless.Microservice.AppHost;

public class EntryPoint {
    public static void Main(params string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.CreateApplicationResource<Nameless_Microservice_Api>(name: "api");
        var postgresdb = builder.CreatePostgresResource().AddDatabase("postgresdb");

        api.WithReference(postgresdb)
           .WaitFor(postgresdb);

        builder.Build().Run();
    }
}