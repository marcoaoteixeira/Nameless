using Aspire.Hosting.JavaScript;
using Microsoft.Extensions.Configuration;

namespace Nameless.Microservices.AppHost.Infrastructure;

public static class DistributedApplicationBuilderExtensions {
    extension(IDistributedApplicationBuilder self) {
        public bool IsHttps => self.Configuration["ASPNETCORE_URLS"]?.Contains("https") == true ||
                               self.Configuration["ASPNETCORE_HTTPS_PORT"] != null;

        public IResourceBuilder<ProjectResource> CreateAspNetCoreResource<TProject>(Action<ResourceOptions>? configure = null)
            where TProject : IProjectMetadata, new() {
            var opts = Apply(configure);

            var result = self.AddProject<TProject>(opts.Name)
                             .WithReplicas(opts.Replicas);

            if (!string.IsNullOrWhiteSpace(opts.HealthCheckUrl)) {
                result.WithHttpHealthCheck(opts.HealthCheckUrl);
            }

            return result;
        }

        public IResourceBuilder<JavaScriptAppResource> CreateAngularResource(Action<AngularResourceOptions>? configure = null) {
            var opts = Apply(configure);

            var result = self.AddJavaScriptApp(
                opts.Name,
                opts.AppDirectory ?? throw new InvalidOperationException("Missing AppDirectory property"),
                opts.RunScriptName
            );

            if (opts.Port > 0) {
                result.WithEnvironment("PORT", opts.Port.ToString());
            }

            result.WithExternalHttpEndpoints();

            if (opts.UseSsl) { result.WithHttpsEndpoint(port: opts.Port, env: "PORT"); }
            else { result.WithHttpEndpoint(port: opts.Port, env: "PORT"); }

            return result;
        }

        public IResourceBuilder<PostgresServerResource> CreatePostgresResource(string name = "postgres") {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var options = self.Configuration
                              .GetSection($"Resources:{name}")
                              .Get<PostgresContainerResourceOptions>();

            if (options is null) {
                throw new InvalidOperationException(
                    $"Resource configuration section '{name}' is missing."
                );
            }

            return self.ConfigurePostgres(name, options)
                       .ConfigurePostgresAdministration(options.PgAdmin);
        }

        private IResourceBuilder<PostgresServerResource> ConfigurePostgres(string name, PostgresContainerResourceOptions opts) {
            // For some reason, setting POSTGRES_USER and POSTGRES_PASSWORD in
            // Aspire does not work with environment variables.
            // Instead, we need to use the AddPostgres method to set these variables.
            var username = opts.Environment.TryGetValue("POSTGRES_USER", out var user)
                ? self.AddParameter("username", valueGetter: () => user ?? string.Empty, secret: true)
                : null;

            var password = opts.Environment.TryGetValue("POSTGRES_PASSWORD", out var pass)
                ? self.AddParameter("password", valueGetter: () => pass ?? string.Empty, secret: true)
                : null;

            var postgres = self.AddPostgres(name, username, password)
                               .WithImage(opts.Image);

            if (!string.IsNullOrWhiteSpace(opts.RegistryUrl)) {
                postgres.WithImageRegistry(opts.RegistryUrl);
            }

            foreach (var variable in opts.Environment) {
                postgres.WithEnvironment(
                    variable.Key,
                    variable.Value
                );
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
        private IResourceBuilder<PostgresServerResource> ConfigurePostgresAdministration(PostgresAdministrationContainerResourceOptions? opts) {
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

    private static TOptions Apply<TOptions>(Action<TOptions>? configure)
        where TOptions : ResourceOptions, new() {
        var result = new TOptions();

        (configure ?? (_ => { })).Invoke(result);

        return result;
    }
}

public record JavaScriptAppOptions {
    public string RunScriptName { get; set; } = "start";
    public int Port { get; set; } = 8080;
    public bool UseSsl { get; set; }
}

public record ResourceOptions {
    public string Name { get; set; } = $"{Guid.CreateVersion7():N}";
    public int Replicas { get; set; } = 1;
    public string? HealthCheckUrl { get; set; }
}

public sealed record AngularResourceOptions : ResourceOptions {
    public string? AppDirectory { get; set; }
    public string RunScriptName { get; set; } = "start";
    public bool UseSsl { get; set; }
    public int Port { get; set; }
}