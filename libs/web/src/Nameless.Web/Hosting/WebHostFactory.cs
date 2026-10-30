using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless.Web.Hosting;

/// <summary>
///     Provides a framework for configuring, building, and running a web
///     application with customizable settings and middleware components.
/// </summary>
public sealed class WebHostFactory : HostFactory {
    private WebHostFactory(string[] args, Assembly[] assemblies, HostFeatureCollection features)
        : base(args, assemblies, features) { }

    /// <summary>
    ///     Creates and builds the web host from the full set of built-in
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
    ///     Nameless.Common and Nameless.Web are always scanned, first. If
    ///     <see langword="null"/>, the assembly that called this method is
    ///     scanned too, so a call from the application's entry point scans
    ///     the application. Pass the assemblies explicitly when calling
    ///     from a shared library that wraps this method; pass an empty
    ///     collection to scan the Nameless libraries only.
    /// </param>
    /// <param name="configureFeatures">
    ///     An optional delegate that configures the ordered collection of
    ///     features used to build the web host. Use it to add, remove,
    ///     replace or configure features. Application services
    ///     and endpoints belong in an application feature added last with
    ///     <see cref="HostFeatureCollection.Add"/>: its
    ///     <see cref="HostFeature.Register"/> runs after every other
    ///     registration (so it can replace them) and its
    ///     <see cref="IWebHostFeature.Use"/> after every other middleware.
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
    ///     Creates and builds the web host from the minimal set of built-in
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

        return new WebHostFactory(args ?? [], supportAssemblies, features).Build();
    }

    /// <summary>
    ///     Builds and returns an <see cref="IHost"/> that represents the
    ///     configured web application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="WebApplication"/> implements <see cref="IHost"/>
    ///         itself, with correct start/run/stop semantics, so it's
    ///         returned directly. This method does not start the web application.
    ///     </para>
    ///     <para>
    ///         <b>Build order</b>:
    ///
    ///         <list type="number">
    ///             <item>
    ///                 <see cref="HostFeature.Register"/> of every feature, in
    ///                 collection order
    ///             </item>
    ///             <item>
    ///                 Build the <see cref="WebApplication"/>
    ///             </item>
    ///             <item>
    ///                 <see cref="IWebHostFeature.Use"/> of every feature, in
    ///                 collection order
    ///             </item>
    ///         </list>
    ///
    ///         The default feature order is defined by
    ///         <see cref="AppHostFeatureCollectionExtensions.CreateDefault"/>.
    ///         A feature added last runs after every built-in in both phases,
    ///         which is where application services and endpoints belong.
    ///     </para>
    ///     <para>
    ///         While building, features log through
    ///         <see cref="HostFeatureContext.Logger"/>, a startup logger
    ///         whose category is the full name of
    ///         <see cref="WebHostFactory"/>. Any exception thrown by a
    ///         feature or by building the application is logged as critical
    ///         and rethrown, except
    ///         <see cref="HostAbortedException"/>. Exceptions thrown by the
    ///         features delegate passed to <see cref="CreateDefault"/> or
    ///         <see cref="CreateMinimal"/> happen before
    ///         the startup logger exists and are not logged.
    ///     </para>
    /// </remarks>
    /// <returns>
    ///     The configured <see cref="WebApplication"/>, as an
    ///     <see cref="IHost"/>.
    /// </returns>
    public override IHost Build() {
        var builder = WebApplication.CreateBuilder(Args);

        // The startup logger is local on purpose: assigning it to the static
        // Log.Logger would break processes that build more than one host.
        // It reads only the minimum level from configuration; configured
        // sinks belong to the application logger, and opening them twice
        // (e.g. a non-shared file) would collide.
        using var serilog = CreateSerilogLogger(builder);
        using var loggerFactory = new SerilogLoggerFactory(serilog);

        var context = new HostFeatureContext(
            assemblies: Assemblies,
            logger: loggerFactory.CreateLogger(typeof(WebHostFactory).FullName!)
        );

        // Snapshot, so both phases see the same features.
        HostFeature[] features = [.. Features];

        WebApplication? app = null;

        try {
            foreach (var feature in features) {
                Log.RegisteringFeature(context.Logger, feature.Name);

                feature.Register(builder, context);
            }

            app = builder.Build();

            foreach (var feature in features) {
                if (feature is not IWebHostFeature webFeature) { continue; }

                Log.UsingFeature(context.Logger, feature.Name);

                webFeature.Use(app, context);
            }

            return app;
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
