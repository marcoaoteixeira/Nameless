﻿using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Endpoint factory interface.
/// </summary>
public interface IEndpointFactory {
    /// <summary>
    ///     Creates an instance of the endpoint for a given descriptor.
    /// </summary>
    /// <param name="descriptor">
    ///     The endpoint descriptor.
    /// </param>
    /// <returns>
    ///     An instance <see cref="IEndpoint"/>.
    /// </returns>
    EndpointCall Create(IEndpointDescriptor descriptor);
}