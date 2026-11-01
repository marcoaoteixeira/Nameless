using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting;

/// <summary>
///     Provides a framework for configuring, building, and running a
///     application with customizable settings components.
/// </summary>
public sealed class WinHostFactory : HostFactory {
    private WinHostFactory(string[] args, Assembly[] assemblies, HostFeatureCollection features)
        : base(args, assemblies, features) { }

    /// <summary>
    ///     Creates and builds the host from the full set of built-in
    ///     features.
    /// </summary>
    /// <remarks>
    ///     The feature collection starts from
    ///     <see cref="AppHostFeatureCollectionExtensions.CreateDefault"/>,
    ///     opt-in features included. Use <see cref="CreateMinimal"/> to
    ///     start from the minimal set instead.
    /// </remarks>
    /// <param name="args">
    ///     The command-line arguments. If <see langword="null"/>, no
    ///     arguments are used.
    /// </param>
    /// <param name="assemblies">
    ///     The assemblies to scan for types, available to every feature
    ///     through <see cref="HostFeatureContext.Assemblies"/>.
    ///     Nameless.Common and Nameless.Windows are always scanned, first. If
    ///     <see langword="null"/>, the assembly that called this method is
    ///     scanned too, so a call from the application's entry point scans
    ///     the application. Pass the assemblies explicitly when calling
    ///     from a shared library that wraps this method; pass an empty
    ///     collection to scan the Nameless libraries only.
    /// </param>
    /// <param name="configureFeatures">
    ///     An optional delegate that configures the ordered collection of
    ///     features used to build the host. Use it to add, remove,
    ///     replace or configure features.
    /// </param>
    /// <returns>
    ///     The configured <see cref="IHost"/>. See <see cref="Build"/>.
    /// </returns>
    // No inlining: GetCallingAssembly must see the caller of this method.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IHost CreateDefault(string[]? args = null, IEnumerable<Assembly>? assemblies = null, Action<HostFeatureCollection>? configureFeatures = null) {
        return CreateFrom(
            args,
            assemblies ?? [Assembly.GetCallingAssembly()],
            HostFeatureCollection.CreateDefault(),
            configureFeatures
        );
    }

    /// <summary>
    ///     Creates and builds the host from the minimal set of built-in
    ///     features.
    /// </summary>
    /// <remarks>
    ///     The feature collection starts from
    ///     <see cref="AppHostFeatureCollectionExtensions.CreateMinimal"/>,
    ///     without the opt-in features; add the ones the application needs.
    ///     Use <see cref="CreateDefault"/> to start from the full set instead.
    /// </remarks>
    /// <inheritdoc cref="CreateDefault" path="/param"/>
    /// <inheritdoc cref="CreateDefault" path="/returns"/>
    // No inlining: GetCallingAssembly must see the caller of this method.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IHost CreateMinimal(string[]? args = null, IEnumerable<Assembly>? assemblies = null, Action<HostFeatureCollection>? configureFeatures = null) {
        return CreateFrom(
            args,
            assemblies ?? [Assembly.GetCallingAssembly()],
            HostFeatureCollection.CreateMinimal(),
            configureFeatures
        );
    }

    private static IHost CreateFrom(string[]? args, IEnumerable<Assembly> assemblies, HostFeatureCollection features, Action<HostFeatureCollection>? configureFeatures) {
        configureFeatures?.Invoke(features);

        // Nameless libraries first: their built-ins (e.g. GlobalExceptionHandler)
        // are found by assembly scan.
        Assembly[] supportAssemblies = [
            .. assemblies.Prepend(typeof(AssemblyMarker).Assembly)
                         .Prepend(typeof(Nameless.AssemblyMarker).Assembly)
                         .Distinct()
        ];

        return new WinHostFactory(args ?? [], supportAssemblies, features).Build();
    }

    /// <summary>
    ///     Builds and returns an <see cref="IHost"/> that represents the
    ///     configured application.
    /// </summary>
    /// <returns>
    ///     The configured <see cref="IHost"/>, as an
    ///     <see cref="IHost"/>.
    /// </returns>
    public override IHost Build() {
        var builder = Host.CreateApplicationBuilder(Args);

        // The startup logger is local on purpose: assigning it to the static
        // Log.Logger would break processes that build more than one host.
        // It reads only the minimum level from configuration; configured
        // sinks belong to the application logger, and opening them twice
        // (e.g. a non-shared file) would collide.
        using var serilog = CreateSerilogLogger(builder);
        using var loggerFactory = new SerilogLoggerFactory(serilog);

        var context = new HostFeatureContext(
            assemblies: Assemblies,
            logger: loggerFactory.CreateLogger(typeof(WinHostFactory).FullName!)
        );

        // Snapshot, so both phases see the same features.
        HostFeature[] features = [.. Features];

        IHost? app = null;

        try {
            // Snapshot, so a feature cannot change the collection being iterated.
            foreach (var feature in features) {
                Log.RegisteringFeature(context.Logger, feature.Name);

                feature.Register(builder, context);
            }

            return app = builder.Build();
        }
        // Tools like "dotnet ef" abort the host on purpose; that's not a
        // failure.
        catch (Exception exception) when (exception is not HostAbortedException) {
            Log.BuildFailure(
                context.Logger,
                $"Web host failed to build. Reason: {exception.Message}",
                exception
            );

            DisposeQuietly(app);

            throw;
        }
    }
}

/// <summary>
///     Host Extension Methods
/// </summary>
public static class HostExtensions {
    extension(IHost self) {
        /// <summary>
        ///     Wraps the current <see cref="IHost"/> in
        ///     a <see cref="WinHost"/> class.
        /// </summary>
        /// <returns>
        ///     A <see cref="WinHost"/> class.
        /// </returns>
        public WinHost Wrap() {
            return new WinHost(self);
        }
    }
}