using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for Entity Framework Core.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers Entity Framework Core services, interceptors, and the
        ///     specified <typeparamref name="TDbContext"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The <see cref="DbContext"/> type to register.</typeparam>
        /// <param name="registration">Optional delegate to configure interceptors and seeder.</param>
        /// <param name="configuration">Optional configuration for <see cref="EntityFrameworkCoreOptions"/>.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterEntityFrameworkCore<TDbContext>(Action<EntityFrameworkCoreRegistration>? registration = null, IConfiguration? configuration = null)
            where TDbContext : DbContext {
            var settings = ActionHelper.FromDelegate(registration);

            self.ConfigureOptions<EntityFrameworkCoreOptions>(configuration);

            self.RegisterInterceptors(settings);
            self.RegisterDataSeeder(settings);

            var configure = (Action<IServiceProvider, DbContextOptionsBuilder>)Delegate.Combine(
                DefaultConfiguration,
                settings.OverrideDbContextConfiguration ?? SqliteDbContextConfiguration
            );

            return settings.UseDbContextFactory
                ? self.AddDbContextFactory<TDbContext>(configure)
                : self.AddDbContext<TDbContext>(configure);
        }

        private void RegisterInterceptors(EntityFrameworkCoreRegistration settings) {
            var service = typeof(IInterceptor);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<IInterceptor>()
                : settings.Interceptors;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Transient(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }

        private void RegisterDataSeeder(EntityFrameworkCoreRegistration settings) {
            var service = typeof(IDatabaseSeeder);
            var implementation = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<IDatabaseSeeder>().SingleOrDefault()
                : settings.DatabaseSeeder;

            if (implementation is null) { return;}

            var descriptor = ServiceDescriptor.Transient(service, implementation);

            self.TryAdd(descriptor);
        }
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

        var databaseSeeder = provider.GetService<IDatabaseSeeder>();
        if (databaseSeeder is not null) {
            builder.UseAsyncSeeding(databaseSeeder.ExecuteAsync)
                   .UseSeeding(databaseSeeder.Execute);
        }
    }
}
