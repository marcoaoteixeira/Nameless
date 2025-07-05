using Projects;

namespace Nameless.Barebones.Aspire.AppHost;

public static class EntryPoint {
    public static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        /*** Configuring Dependencies ***/
        var postgres = builder.AddPostgres("postgres")
                              .WithPgWeb()
                              .AddDatabase("barebones-db");

        /*** Configuring API ***/
        var api = builder.AddProject<Nameless_Barebones_Api>("api")
                         .WithExternalHttpEndpoints()
                         .WithReference(postgres)
                         .WaitFor(postgres);

        /*** Configuring Frontend ***/
        builder.AddProject<Nameless_Barebones_Frontend>("frontend")
               .WithReference(api)
               .WaitFor(api);

        /*** Startup Aspire Dashboard ***/
        builder.Build().Run();
    }
}