using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Entity Framework Core Host Feature
/// </summary>
public class EntityFrameworkCoreHostFeature<TDbContext> : HostFeature<EntityFrameworkCoreFeatureOptions>
    where TDbContext : DbContext {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);
        var services = builder.Services;

        services.ConfigureOptions<EntityFrameworkCoreOptions>(builder.Configuration);

        RegisterInterceptors(services, options);
        RegisterDataSeeders(services, options);
        RegisterEntityTypeConfigurations(services, options);

        var configureDbContext = (Action<IServiceProvider, DbContextOptionsBuilder>)Delegate.Combine(
            ConfigureDefaultEntityFrameworkCore,
            options.DbContextConfiguration ?? SqliteDbContextConfiguration
        );

        _ = options.UseDbContextFactory
            ? services.AddDbContextFactory<TDbContext>(configureDbContext)
            : services.AddDbContext<TDbContext>(configureDbContext);
    }
    
    private static void RegisterInterceptors(IServiceCollection self, EntityFrameworkCoreFeatureOptions options) {
        self.TryAddEnumerable(options.Interceptors.Select(
            implementation => ServiceDescriptor.Transient(typeof(IInterceptor), implementation)
        ));
    }

    private static void RegisterDataSeeders(IServiceCollection self, EntityFrameworkCoreFeatureOptions options) {
        const string Key = "::database:seeder::dd499dc3-f896-4030-ba59-00f8bb2647e9";

        self.TryAddTransient<DatabaseSeederAggregator>(provider => new DatabaseSeederAggregator(
            seeders: provider.GetKeyedServices<IDatabaseSeeder>(Key),
            logger: provider.GetLogger<DatabaseSeederAggregator>()
        ));

        self.TryAddEnumerable(
            options.DatabaseSeeders.Select(
                implementation => ServiceDescriptor.KeyedTransient(
                    typeof(IDatabaseSeeder), Key, implementation
                )
            )
        );
    }

    private static void RegisterEntityTypeConfigurations(IServiceCollection self, EntityFrameworkCoreFeatureOptions options) {
        options.EntityTypeConfigurationCallback?.Invoke(self);
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

    private static void ConfigureDefaultEntityFrameworkCore(IServiceProvider provider, DbContextOptionsBuilder builder) {
        builder.AddInterceptors(
            provider.GetServices<IInterceptor>()
        );

        var aggregator = provider.GetRequiredService<DatabaseSeederAggregator>();
        builder.UseAsyncSeeding(aggregator.ExecuteAsync)
               .UseSeeding(aggregator.Execute);
    }
}