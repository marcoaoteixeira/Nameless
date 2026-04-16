using Microsoft.Extensions.Configuration;

namespace Nameless.Microservice.AppHost.Infrastructure;

public static class DistributedApplicationBuilderExtensions {
    extension(IDistributedApplicationBuilder self) {
        public IResourceBuilder<ProjectResource> CreateApplicationResource<TProject>(string name = "app", int replicas = 1)
            where TProject : IProjectMetadata, new() {
            return self.AddProject<TProject>(name)
                       .WithReplicas(replicas)
                       .WithHttpHealthCheck("/health");
        }

        public IResourceBuilder<PostgresServerResource> CreatePostgresResource(string name = "postgres") {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var options = self.Configuration
                              .GetSection($"Resources:{name}")
                              .Get<PostgresResourceOptions>();

            if (options is null) {
                throw new InvalidOperationException(
                    $"Resource configuration section '{name}' is missing."
                );
            }

            return self.ConfigurePostgres(name, options)
                       .ConfigurePostgresAdministration(options.PgAdmin);
        }

        private IResourceBuilder<PostgresServerResource> ConfigurePostgres(string name, PostgresResourceOptions opts) {
            var postgres = self.AddPostgres(name).WithImage(opts.Image);

            if (!string.IsNullOrWhiteSpace(opts.RegistryUrl)) {
                postgres.WithImageRegistry(opts.RegistryUrl);
            }

            postgres.WithLifetime(
                opts.IsPersistent
                    ? ContainerLifetime.Persistent
                    : ContainerLifetime.Session
            );

            // Data volumes don't work on ACA for Postgres, so only add when running local
            if (self.ExecutionContext.IsRunMode) {
                postgres.WithDataVolume(opts.VolumeName);
            }

            return postgres.ConfigurePostgresAdministration(opts.PgAdmin);
        }
    }

    extension(IResourceBuilder<PostgresServerResource> self) {
        private IResourceBuilder<PostgresServerResource> ConfigurePostgresAdministration(PostgresAdministrationResourceOptions? opts) {
            if (opts is null) { return self; }

            return self.WithPgAdmin(pgAdmin => {
                pgAdmin.WithImage(opts.Image);

                if (!string.IsNullOrWhiteSpace(opts.RegistryUrl)) {
                    pgAdmin.WithImageRegistry(opts.RegistryUrl);
                }

                foreach (var variable in opts.Environment) {
                    pgAdmin.WithEnvironment(
                        variable.Key,
                        variable.Value
                    );
                }

                pgAdmin.WithLifetime(
                    opts.IsPersistent
                        ? ContainerLifetime.Persistent
                        : ContainerLifetime.Session
                );

                pgAdmin.WithHostPort(opts.HostPort);
            });
        }
    }
}
