using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Web.Endpoints.Infrastructure;

namespace Nameless.Web.Endpoints.Interception;
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Configures minimal endpoint services for the application,
    ///     including OpenAPI, versioning, and all implemented endpoints.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    /// <remarks>
    ///     This method sets up essential services required for minimal
    ///     endpoint functionality, including OpenAPI documentation, API
    ///     versioning, and core application services.
    /// 
    ///     Use the <paramref name="configure"/> parameter to customize
    ///     endpoint options, such as enabling or disabling specific features.
    /// </remarks>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="useInterception">
    ///     Whether it should use endpoint interception.
    /// </param>
    /// <param name="configure">
    ///     An optional delegate to configure <see cref="EndpointOptions"/> for
    ///     customizing endpoint behavior. If not provided, default options are
    ///     used.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/>
    ///     so other actions can be chained.
    /// </returns>
    /// <remarks>
    ///     Interception is an experimental feature that might or not work
    ///     properly, so use it at your own risk.
    /// </remarks>
    public static THostApplicationBuilder RegisterMinimalEndpoints<THostApplicationBuilder>(this THostApplicationBuilder self, bool useInterception, Action<EndpointOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new EndpointOptions();

        innerConfigure(options);

        self.RegisterMinimalEndpoints(innerConfigure);

        if (!useInterception) {
            return self;
        }

        // Replace the IEndpointFactory service
        self.Services.Replace(new ServiceDescriptor(
            serviceType: typeof(IEndpointFactory),
            implementationType: typeof(EndpointFactoryWithInterception),
            lifetime: ServiceLifetime.Singleton
        ));

        self.Services.TryAddSingleton<IEndpointWrapperGenerator, EndpointWrapperGenerator>();
        self.Services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
        //var interceptors = options.Assemblies
        //                          .GetImplementations(typeof(EndpointInterceptorBase))
        //                          .Where(type => !type.IsGenericTypeDefinition)
        //                          .Select(interceptor => new ServiceDescriptor(interceptor, interceptor, ServiceLifetime.Transient));

        //foreach (var interceptor in interceptors) {
        //    self.Services.TryAdd(interceptor);
        //}

        return self;
    }
}
