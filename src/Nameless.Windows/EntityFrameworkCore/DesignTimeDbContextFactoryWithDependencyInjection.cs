using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Nameless.Windows.EntityFrameworkCore;

/// <summary>
///     A design time database context factory for Windows applications.
///     This is necessary to use the EF Core CLI tool
/// </summary>
public abstract class DesignTimeDbContextFactoryWithDependencyInjection<TDbContext> : IDesignTimeDbContextFactory<TDbContext>, IDisposable
    where TDbContext : DbContext {

    private readonly Lazy<IServiceProvider> _serviceProvider;

    private bool _disposed;

    protected static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                                               Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                                               "Development";

    public IServiceProvider Services => _serviceProvider.Value;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="DesignTimeDbContextFactoryWithDependencyInjection{TDbContext}"/>
    ///     class.
    /// </summary>
    protected DesignTimeDbContextFactoryWithDependencyInjection() {
        _serviceProvider = new Lazy<IServiceProvider>(CreateServiceProvider);
    }

    /// <inheritdoc />
    public abstract TDbContext CreateDbContext(string[] args);

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Configures the dependency injection services.
    /// </summary>
    /// <param name="services">
    ///     The service collection.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }

    /// <summary>
    ///     Disposes the class instance.
    /// </summary>
    /// <param name="disposing">
    ///     Whether it should dispose managed resources.
    /// </param>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            if (Services is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }

    private ServiceProvider CreateServiceProvider() {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        services.TryAddSingleton(configuration);
        services.AddLogging(logging => {
            logging.AddConsole();
        });

        ConfigureServices(services, configuration);

        return services.BuildServiceProvider();
    }

    private static IConfiguration CreateConfiguration() {
        return new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("AppSettings.json", optional: true)
               .AddJsonFile($"AppSettings.{EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables()
               .Build();
    }
}
