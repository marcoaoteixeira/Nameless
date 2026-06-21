using Projects;

namespace Nameless.Microservices.AppHost;

public class EntryPoint {
    public static void Main(string[] args) {
        var builder = DistributedApplication.CreateBuilder(args);

        var postgres = builder.CreatePostgresResource(static postgres => {
            postgres.RegistryUrl = "devopsenhesacontainerregistry.azurecr.io/cache/docker.io/library";
            postgres.Image = "postgres:17-alpine";
            postgres.Name = "postgres";
            postgres.HostPort = 5432;
            postgres.IsPersistent = true;
            postgres.DatabaseName = "microservices";
            postgres.ConnectionStringName = "postgresdb";
            postgres.Username = "user";
            postgres.Password = "password";

            postgres.UsePgAdmin = static pgAdmin => {
                pgAdmin.Image = "elestio/pgadmin:REL-9_15";
                pgAdmin.HostPort = 6543;
                pgAdmin.IsPersistent = true;
                pgAdmin.Environment = new Dictionary<string, string?>
                {
                    { "PGADMIN_DEFAULT_EMAIL", "pgamin@pgadmin.com" },
                    { "PGADMIN_DEFAULT_PASSWORD", "pgamin" }
                };
            };
        });

        var api = builder.CreateAspNetCoreResource<Nameless_Microservices_Api>(api => {
            api.Name = "api";
            api.HealthCheckUrl = "/health";
        });

        var bff = builder.CreateAspNetCoreResource<Nameless_Microservices_Bff>(bff => {
            bff.Name = "bff";
            bff.HealthCheckUrl = "/health";
        });

        var worker = builder.CreateAspNetCoreResource<Nameless_Microservices_Worker>(worker => {
            worker.Name = "worker";
            worker.HealthCheckUrl = "/health";
        });

        var frontend = builder.CreateJavaScriptAppResource(
            appDirectory: "../Enhesa.PRO.Microservices.Frontend",
            configure: frontend => frontend.Name = "frontend"
        );

        api.WithReference(postgres).WaitFor(postgres);
        worker.WithReference(postgres).WaitFor(postgres);

        bff.WithReference(api).WaitFor(api);
        frontend.WithReference(bff).WaitFor(bff);

        builder.Build().Run();
    }
}
