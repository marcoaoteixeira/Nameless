using Nameless.Microservices.AppHost.Infrastructure;
using Projects;

namespace Nameless.Microservices.AppHost;

public static class EntryPoint {
    public static void Main(params string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.CreateAspNetCoreResource<Nameless_Microservices_Api>(configure => {
            configure.Name = "api";
            configure.Replicas = 3;
        });

        var frontend = builder.CreateAngularResource(configure => {
            configure.Name = "frontend";
            configure.AppDirectory = "../Nameless.Microservices.Frontend";
            configure.UseSsl = builder.IsHttps;
            configure.Port = builder.IsHttps ? 8443 : 8080;
        });

        #pragma warning disable ASPIREJAVASCRIPT001
        frontend.PublishAsStaticWebsite(apiPath: "/api", apiTarget: api);
        #pragma warning restore ASPIREJAVASCRIPT001

        var postgres = builder.CreatePostgresResource().AddDatabase("postgresdb");

        api.WithReference(postgres)
           .WaitFor(postgres);

        frontend.WithReference(api)
                .WaitFor(api);

        builder.Build().Run();
    }
}