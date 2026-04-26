using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.EntityFrameworkCore;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterEntityFrameworkCore<TDbContext>(Action<EntityFrameworkCoreRegistration>? registration = null, IConfiguration? configuration = null)
            where TDbContext : DbContext {
            var settings = ActionHelper.FromDelegate(registration);

            self.ConfigureOptions<EntityFrameworkCoreOptions>(configuration);

            // register interceptors, they might need injection.
            self.TryAddEnumerable(
                descriptors: CreateInterceptorServiceDescriptors(settings)
            );

            // register database seeder
            self.TryAdd(
                descriptor: CreateDatabaseSeederServiceDescriptor(settings)
            );
            
            self.AddDbContext<TDbContext>(
                (Action<IServiceProvider, DbContextOptionsBuilder>)Delegate.Combine(
                    DefaultConfiguration,
                    settings.OverrideDbContextConfiguration ?? SqliteDbContextConfiguration
                )
            );

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateInterceptorServiceDescriptors(EntityFrameworkCoreRegistration settings) {
        var service = typeof(IInterceptor);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan<IInterceptor>()
            : settings.Interceptors;

        return implementations.Select(
            implementation => ServiceDescriptor.Transient(service, implementation)
        );
    }

    private static ServiceDescriptor CreateDatabaseSeederServiceDescriptor(EntityFrameworkCoreRegistration settings) {
        var service = typeof(IDatabaseSeeder);
        var implementation = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan<IDatabaseSeeder>().SingleOrDefault()
            : settings.DatabaseSeeder;

        return implementation is not null
            ? ServiceDescriptor.Transient(service, implementation)
            : ServiceDescriptor.Singleton(NullDatabaseSeeder.Instance);
    }

    private static void SqliteDbContextConfiguration(IServiceProvider provider, DbContextOptionsBuilder builder) {
        // NOTE: If using Aspire, it's possible to retrieve the
        // connection string being used by the database resource in
        // Aspire. Aspire will automatically inject the connection
        // strings in the "ConnectionStrings" configuration section
        // for the given resource name.
        var options = provider.GetOptions<EntityFrameworkCoreOptions>().Value;
        var connStr = provider.GetRequiredService<IConfiguration>()
                              .GetConnectionString(options.ConnectionStringName);

        builder.UseSqlite(connStr);
    }

    private static void DefaultConfiguration(IServiceProvider provider, DbContextOptionsBuilder builder) {
        var interceptors = provider.GetServices<IInterceptor>();
        builder.AddInterceptors(interceptors);

        var databaseSeeder = provider.GetRequiredService<IDatabaseSeeder>();
        builder.UseAsyncSeeding(databaseSeeder.ExecuteAsync)
               .UseSeeding(databaseSeeder.Execute);
    }
}
