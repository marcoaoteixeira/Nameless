using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.WPF;

/// <summary>
///     Factory class to create a <see cref="IHost" /> instance.
/// </summary>
public sealed class HostFactory {
    public const string EnvironmentKey = "--env:";

    private readonly string[] _args;

    private Action<IServiceCollection, IConfiguration> _configure;
    private Action<IServiceProvider> _onStartup;
    private Action<IServiceProvider> _onTearDown;

    // We want to force the usage of the static Create method
    private HostFactory(string[] args) {
        _args = args;
        _configure = (_, _) => { };
        _onStartup = _ => { };
        _onTearDown = _ => { };
    }

    /// <summary>
    ///     Creates a new instance of <see cref="HostFactory" />.
    /// </summary>
    /// <param name="args">
    ///     The command line arguments.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="HostFactory" />.
    /// </returns>
    public static HostFactory Create(params string[] args) {
        return new HostFactory(args);
    }

    /// <summary>
    ///     Configures the services for the host.
    /// </summary>
    /// <param name="configure">
    ///     The action to configure the services.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="HostFactory" /> so other
    ///     actions can be chained.
    /// </returns>
    public HostFactory ConfigureServices(Action<IServiceCollection, IConfiguration> configure) {
        _configure = configure;

        return this;
    }

    /// <summary>
    ///     Executes the <paramref name="callback"/> after the application
    ///     has started.
    /// </summary>
    /// <param name="callback">
    ///     The startup callback.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="HostFactory" /> so other
    ///     actions can be chained.
    /// </returns>
    public HostFactory OnStartup(Action<IServiceProvider> callback) {
        _onStartup = callback;

        return this;
    }

    /// <summary>
    ///     Executes the <paramref name="callback"/> before the application
    ///     stops.
    /// </summary>
    /// <param name="callback">
    ///     The startup callback.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="HostFactory" /> so other
    ///     actions can be chained.
    /// </returns>
    public HostFactory OnTearDown(Action<IServiceProvider> callback) {
        _onTearDown = callback;

        return this;
    }

    /// <summary>
    ///     Builds the <see cref="IHost" /> instance.
    /// </summary>
    /// <returns>
    ///     The built <see cref="IHost" /> instance.
    /// </returns>
    public IHost Build() {
        var environment = GetEnvironment();
        var host = Host.CreateDefaultBuilder(_args)
                       .ConfigureHostConfiguration(builder => {
                           builder
                               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                               .AddJsonFile(path: "AppSettings.json", optional: true, reloadOnChange: true)
                               .AddJsonFile(path: $"AppSettings.{environment}.json", optional: true, reloadOnChange: true)
                               .AddEnvironmentVariables();
                       })
                       .ConfigureServices((ctx, services) => _configure(services, ctx.Configuration))
                       .Build();

        RegisterHostApplicationLifetimeEvents(host);

        return host;
    }

    private void RegisterHostApplicationLifetimeEvents(IHost host) {
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(() => _onStartup(host.Services));
        lifetime.ApplicationStopping.Register(() => _onTearDown(host.Services));
    }

    private string GetEnvironment() {
        var environment = _args.SingleOrDefault(
            arg => arg.StartsWith(EnvironmentKey, StringComparison.OrdinalIgnoreCase)
        );

        return environment?.Split(CoreConstants.Separators.Colon).Last() ?? "Development";
    }
}