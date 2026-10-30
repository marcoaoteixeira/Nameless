using Nameless.Hosting;
using Nameless.Mediator;
using Nameless.Resilience;
using Nameless.Validation.FluentValidation;
using Nameless.Web.Hosting.Features;
using Nameless.Workers;

namespace Nameless.Web.Hosting;

internal static class AppHostFeatureCollectionExtensions {
    extension(HostFeatureCollection self) {
        /// <summary>
        ///     Creates a new collection with the built-in features in the order
        ///     required by the ASP.NET Core request pipeline.
        /// </summary>
        /// <remarks>
        ///     Features with middlewares come first, in pipeline order. Features
        ///     that only register services come last; their position does not
        ///     affect the request pipeline.
        /// </remarks>
        /// <returns>
        ///     A new <see cref="HostFeatureCollection"/> instance.
        /// </returns>
        public static HostFeatureCollection CreateDefault() {
            return new HostFeatureCollection()
                   .Add(new CommonWebHostFeature())
                   .Add(new ExceptionHandlingWebHostFeature())
                   .Add(new SerilogWebHostFeature())
                   .Add(new HttpRequestCorrelationWebHostFeature())

                   // Important: routing must come before any feature that relies on
                   // endpoint metadata, like CORS, rate limiter or authorization.
                   .Add(new EndpointRoutingWebHostFeature())

                   .Add(new HealthCheckWebHostFeature())
                   .Add(new HttpsRedirectionWebHostFeature())
                   .Add(new CorsWebHostFeature())
                   .Add(new RateLimiterWebHostFeature())
                   .Add(new OutputCacheWebHostFeature())
                   .Add(new RequestTimeoutWebHostFeature())
                   .Add(new AuthWebHostFeature())
                   .Add(new AntiforgeryWebHostFeature())
                   .Add(new OpenApiWebHostFeature())
                   .Add(new ScalarWebHostFeature())

                   // Register-only features.
                   .Add(new DataProtectionWebHostFeature())
                   .Add(new MediatorHostFeature())
                   .Add(new OpenTelemetryWebHostFeature())
                   .Add(new PeriodicWorkerHostFeature())
                   .Add(new ResilienceHostFeature())
                   .Add(new ServiceDiscoveryWebHostFeature())
                   .Add(new FluentValidationHostFeature());
        }

        /// <summary>
        ///     Creates a new collection with the minimal built-in features in
        ///     the order required by the ASP.NET Core request pipeline.
        /// </summary>
        /// <remarks>
        ///     Features with middlewares come first, in pipeline order. Features
        ///     that only register services come last; their position does not
        ///     affect the request pipeline.
        /// </remarks>
        /// <returns>
        ///     A new <see cref="HostFeatureCollection"/> instance.
        /// </returns>
        public static HostFeatureCollection CreateMinimal() {
            return new HostFeatureCollection()
                   .Add(new CommonWebHostFeature())
                   .Add(new ExceptionHandlingWebHostFeature())
                   .Add(new SerilogWebHostFeature())
                   .Add(new HttpRequestCorrelationWebHostFeature())

                   // Important: routing must come before any feature that relies on
                   // endpoint metadata, like CORS, rate limiter or authorization.
                   .Add(new EndpointRoutingWebHostFeature())

                   .Add(new HealthCheckWebHostFeature())
                   .Add(new CorsWebHostFeature())
                   .Add(new AuthWebHostFeature())
                   .Add(new OpenApiWebHostFeature())
                   .Add(new ScalarWebHostFeature())

                   // Register-only features.
                   .Add(new MediatorHostFeature())
                   .Add(new OpenTelemetryWebHostFeature())
                   .Add(new ResilienceHostFeature())
                   .Add(new ServiceDiscoveryWebHostFeature())
                   .Add(new FluentValidationHostFeature());
        }
    }
}
