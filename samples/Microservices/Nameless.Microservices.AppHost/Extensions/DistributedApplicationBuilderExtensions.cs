using Aspire.Hosting.JavaScript;
using Nameless.Microservices.AppHost.Infrastructure;
using Nameless.Microservices.AppHost.Options;

namespace Nameless.Microservices.AppHost;

public static class DistributedApplicationBuilderExtensions {
    extension(IDistributedApplicationBuilder self) {
        public bool IsHttps => self.Configuration["ASPNETCORE_URLS"]?.Contains("https") == true ||
                               self.Configuration["ASPNETCORE_HTTPS_PORT"] != null;

        public IResourceBuilder<ProjectResource> CreateAspNetCoreResource<TProject>(Action<AspNetCoreResourceOptions>? configure = null)
            where TProject : IProjectMetadata, new() {
            var resourceOpts = Apply(configure);

            var name = resourceOpts.Name ?? NameGenerator.Generate(separator: string.Empty);
            var replicas = resourceOpts.Replicas > 0 ? resourceOpts.Replicas : 1;

            return self
                .AddProject<TProject>(name)
                .WithReplicas(replicas)
                .WithHttpHealthCheck(resourceOpts.HealthCheckUrl);
        }

        public IResourceBuilder<JavaScriptAppResource> CreateJavaScriptAppResource(string appDirectory, Action<JavaScriptAppResourceOptions>? configure = null) {
            var resourceOpts = Apply(configure);

            var name = resourceOpts.Name ?? NameGenerator.Generate(separator: string.Empty);

            // var replicas = resourceOpts.Replicas > 0 ? resourceOpts.Replicas : 1;
            // TODO: Check how to use replicas with Nginx

            var result = self.AddJavaScriptApp(name, appDirectory, resourceOpts.RunScriptName ?? "start");

            var port = resourceOpts.Port > 0 ? (int?)resourceOpts.Port : null;
            if (resourceOpts.UseSsl) {
                result
                    .WithHttpsEndpoint(port: port, env: "PORT")
                    .WithDeveloperCertificateTrust(trust: true);
            }
            else { result.WithHttpEndpoint(port: port, env: "PORT"); }

            return result.WithHttpHealthCheck(resourceOpts.HealthCheckUrl);
        }

        public IResourceBuilder<PostgresDatabaseResource> CreatePostgresResource(Action<PostgresResourceOptions>? configure = null) {
            var resourceOpts = Apply(configure);

            // Aspire Postgres can't use environment variables to configure user/password
            // Instead, it relies on Aspire parameters infrastructure.
            var username = !string.IsNullOrWhiteSpace(resourceOpts.Username)
                ? self.AddParameter("PostgresUser", valueGetter: () => resourceOpts.Username, secret: true)
                : null;

            var password = !string.IsNullOrWhiteSpace(resourceOpts.Password)
                ? self.AddParameter("PostgresPassword", valueGetter: () => resourceOpts.Password, secret: true)
                : null;

            var postgresName = resourceOpts.Name ?? NameGenerator.Generate(separator: string.Empty);
            var connectionStringName = resourceOpts.ConnectionStringName ?? $"{postgresName}Db";
            var port = resourceOpts.HostPort > 0 ? (int?)resourceOpts.HostPort : null;

            return self
                .AddPostgres(postgresName, username, password, port)
                .ConfigurePostgresResource(resourceOpts, self.ExecutionContext.IsRunMode)
                .ConfigurePostgresAdministratorResource(resourceOpts.UsePgAdmin)
                .AddDatabase(connectionStringName, resourceOpts.DatabaseName);
        }
    }

    extension(IResourceBuilder<PostgresServerResource> self) {
        private IResourceBuilder<PostgresServerResource> ConfigurePostgresResource(PostgresResourceOptions opts, bool isRunMode) {
            ArgumentException.ThrowIfNullOrWhiteSpace(opts.Image);

            self.WithImage(opts.Image);

            if (!string.IsNullOrWhiteSpace(opts.RegistryUrl)) {
                self.WithImageRegistry(opts.RegistryUrl);
            }

            self.WithLifetime(
                opts.IsPersistent
                    ? ContainerLifetime.Persistent
                    : ContainerLifetime.Session
            );

            foreach (var kvp in opts.Environment) {
                self.WithEnvironment(kvp.Key, kvp.Value);
            }

            // Data volumes don't work on ACA for Postgres so only add when running
            if (isRunMode) {
                self.WithDataVolume(opts.VolumeName);
            }

            return self;
        }

        private IResourceBuilder<PostgresServerResource> ConfigurePostgresAdministratorResource(Action<PostgresAdministratorResourceOptions>? configure) {
            if (configure is null) { return self; }

            var opts = Apply(configure);

            ArgumentException.ThrowIfNullOrWhiteSpace(opts.Image);

            return self.WithPgAdmin(pgAdmin => {
                var port = opts.HostPort > 0 ? (int?)opts.HostPort : null;

                pgAdmin
                    .WithImageRegistry(opts.RegistryUrl)
                    .WithImage(opts.Image)
                    .WithHostPort(port)
                    .WithLifetime(
                        opts.IsPersistent
                            ? ContainerLifetime.Persistent
                            : ContainerLifetime.Session
                    );

                foreach (var kvp in opts.Environment) {
                    pgAdmin.WithEnvironment(kvp.Key, kvp.Value);
                }
            });
        }
    }

    private static TProjectResourceOptions Apply<TProjectResourceOptions>(Action<TProjectResourceOptions>? configure)
        where TProjectResourceOptions : ResourceOptions, new() {
        var opts = new TProjectResourceOptions();

        (configure ?? (_ => { })).Invoke(opts);

        return opts;
    }
}