using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Microservices.Infrastructure.EntityFrameworkCore;
using Nameless.Microservices.Infrastructure.HealthChecks.Database;

namespace Nameless.Microservices.Configs;

public static class EntityFrameworkCoreConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder RegisterEntityFrameworkCore<TDbContext>(Action<EntityFrameworkCoreRegistrationSettings> registration)
            where TDbContext : DbContext {
            self.Services.Configure<EntityFrameworkCoreOptions>(
                self.Configuration
            );

            return self.InnerRegisterEntityFrameworkCore<TDbContext>(registration);
        }

        public WebApplicationBuilder RegisterEntityFrameworkCore<TDbContext>(Action<EntityFrameworkCoreRegistrationSettings> registration, Action<EntityFrameworkCoreOptions> configure)
            where TDbContext : DbContext {
            self.Services.Configure(configure);

            return self.InnerRegisterEntityFrameworkCore<TDbContext>(registration);
        }

        private WebApplicationBuilder InnerRegisterEntityFrameworkCore<TDbContext>(Action<EntityFrameworkCoreRegistrationSettings> registration)
            where TDbContext : DbContext {
            var settings = ActionHelper.FromDelegate(registration);

            // register interceptors, they might need injection.
            self.Services.TryAddEnumerable(
                descriptors: CreateInterceptorDescriptors(settings)
            );

            // register database seeder
            self.Services.TryAdd(
                descriptor: CreateDatabaseSeederDescriptor(settings)
            );

            // register health check
            self.RegisterDatabaseHealthCheck();

            self.Services.AddDbContext<TDbContext>((provider, builder) => {
                // NOTE: If using Aspire, it's possible to retrieve the
                // connection string being used by the database resource in
                // Aspire. Aspire will automatically inject the connection
                // strings in the "ConnectionStrings" configuration section
                // for the given resource name.
                var options = provider.GetOptions<EntityFrameworkCoreOptions>().Value;
                var connStr = provider.GetRequiredService<IConfiguration>()
                                      .GetConnectionString(options.ConnectionStringName);

                builder = options.Connector switch {
                    EntityFrameworkCoreConnector.Sqlite => builder.UseSqlite(connStr),
                    EntityFrameworkCoreConnector.PostgreSQL => builder.UseNpgsql(connStr),
                    _ => throw new InvalidOperationException($"Missing implementation for connector '{options.Connector}'")
                };

                var interceptors = provider.GetServices<IInterceptor>();
                builder.AddInterceptors(interceptors);

                var databaseSeeder = provider.GetRequiredService<IDatabaseSeeder>();
                builder.UseAsyncSeeding(databaseSeeder.ExecuteAsync)
                       .UseSeeding(databaseSeeder.Execute);

            });

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateInterceptorDescriptors(EntityFrameworkCoreRegistrationSettings settings) {
        var service = typeof(IInterceptor);

        return settings.Interceptors.Select(implementation
            => ServiceDescriptor.Transient(
                service,
                implementation
            )
        );
    }

    private static ServiceDescriptor CreateDatabaseSeederDescriptor(EntityFrameworkCoreRegistrationSettings settings) {
        var service = typeof(IDatabaseSeeder);
        var implementation = settings.DatabaseSeeder;

        return implementation is not null
            ? ServiceDescriptor.Transient(service, implementation)
            : ServiceDescriptor.Singleton(NullDatabaseSeeder.Instance);
    }
}
