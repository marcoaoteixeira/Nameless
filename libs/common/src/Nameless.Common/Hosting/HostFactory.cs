using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System.Reflection;

namespace Nameless.Hosting;

/// <summary>
///     Provides a framework for configuring, building, and running an
///     application with customizable settings.
/// </summary>
/// <remarks>
///     The <see cref="HostFactory"/> class
///     encapsulates the setup and execution of an application using a builder
///     pattern. The host is composed of an ordered
///     <see cref="HostFeatureCollection"/>. This class is intended to
///     simplify the initialization and management of applications by
///     centralizing configuration and startup logic.
/// </remarks>
public abstract class HostFactory {
    /// <summary>
    ///     Gets the host arguments.
    /// </summary>
    protected string[] Args { get; }

    /// <summary>
    ///     Gets the assemblies used when scanning for types.
    /// </summary>
    protected Assembly[] Assemblies { get; }

    /// <summary>
    ///     Gets the host features.
    /// </summary>
    protected HostFeatureCollection Features { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="HostFactory"/> class.
    /// </summary>
    /// <param name="args">
    ///     The arguments for the host application.
    /// </param>
    /// <param name="assemblies">
    ///     The assemblies used when scanning for types.
    /// </param>
    /// <param name="features">
    ///     A collection of features.
    /// </param>
    protected HostFactory(string[] args, Assembly[] assemblies, HostFeatureCollection features) {
        Args = args;
        Assemblies = assemblies;
        Features = features;
    }

    /// <summary>
    ///     Builds and returns an <see cref="IHost"/> that represents the
    ///     configured application.
    /// </summary>
    /// <returns>
    ///     The configured <see cref="IHost"/> instance.
    /// </returns>
    public abstract IHost Build();

    /// <summary>
    ///     Creates a Serilog logger at the startup.
    /// </summary>
    /// <param name="builder">
    ///     The host application builder instance.
    /// </param>
    /// <returns>
    ///     A Serilog logger instance.
    /// </returns>
    protected static Logger CreateSerilogLogger(IHostApplicationBuilder builder) {
        // Keys keep their full path ("Serilog:MinimumLevel:..."), so Serilog
        // reads them as if they came from the original configuration.
        var settings = builder.Configuration
            .GetSection("Serilog:MinimumLevel")
            .AsEnumerable();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .CreateLogger();
    }

    /// <summary>
    ///     Disposes the host quietly.
    /// </summary>
    /// <param name="host">
    ///     The host instance.
    /// </param>
    protected static void DisposeQuietly(IHost? host) {
        // Flushes the application logger and releases the root provider.
        try { host?.Dispose(); }
        catch { /* Never hide the original exception. */ }
    }
}
