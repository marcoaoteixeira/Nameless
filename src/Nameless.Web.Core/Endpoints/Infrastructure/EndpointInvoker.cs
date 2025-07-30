﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Helpers;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     The EndpointInvoker class provides functionality to invoke endpoints.
/// </summary>
internal static class EndpointInvoker {
    /// <summary>
    ///     Invokes an endpoint asynchronously using the provided HttpContext
    ///     and factory.
    /// </summary>
    /// <param name="httpContext">
    ///     The HttpContext to use for the invocation.
    /// </param>
    /// <param name="factory">
    ///     The factory to create the endpoint instance.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if the <see cref="IEndpointFeature"/> is not available or
    ///     the <see cref="EndpointDescriptorMetadata"/> is not available in the feature.
    /// </exception>
    internal static Task InvokeAsync(HttpContext httpContext, IEndpointFactory factory) {
        var descriptor = httpContext.GetEndpointDescriptorMetadata().Descriptor;
        var endpointInfo = factory.Create(descriptor);

        var requestDelegate = RequestDelegateFactory.Create(
            methodInfo: endpointInfo.Handler,
            targetFactory: _ => endpointInfo.Instance,
            options: new RequestDelegateFactoryOptions {
                RouteParameterNames = RouteHelper.GetRouteParameters(descriptor.RoutePattern)
            }
        );

        return requestDelegate.RequestDelegate.Invoke(httpContext);
    }
}
