using System.Reflection;
using Microsoft.AspNetCore.Http;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Extension methods for <see cref="IEndpointDescriptor"/>.
/// </summary>
public static class EndpointDescriptorExtensions {
    /// <summary>
    ///     Retrieves the name for the endpoint or the default value for
    ///     the name. The default value is a composition between the endpoint
    ///     type name and the version number.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/> instance.
    /// </param>
    /// <returns>
    ///     The name of the endpoint, or the default value.
    /// </returns>
    public static string GetNameOrDefault(this IEndpointDescriptor self) {
        return string.IsNullOrWhiteSpace(self.Name)
            ? $"{self.EndpointType.Name}_v{self.Version.Number}"
            : self.Name;
    }

    /// <summary>
    ///     Retrieves the endpoint action method.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/>.
    /// </param>
    /// <returns>
    ///     The action for the endpoint.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="self"/> or;
    ///     if <see cref="IEndpointDescriptor.EndpointType"/> or
    ///     <see cref="IEndpointDescriptor.ActionName"/> inside
    ///     <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <see cref="IEndpointDescriptor.ActionName"/> inside
    ///     <paramref name="self"/> is empty or only whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if method not found in endpoint type or
    ///     return type for the method is not assignable
    ///     from <see cref="Task{TResult}"/> where TResult
    ///     is <see cref="IResult"/>.
    /// </exception>
    public static MethodInfo GetEndpointHandler(this IEndpointDescriptor self) {
        Guard.Against.Null(self);

        var metadata = self.GetAdditionalMetadata<EndpointHandlerMetadata>()
                           .SingleOrDefault();
        if (metadata is null) {
            throw new InvalidOperationException("Endpoint descriptor missing handler metadata.");
        }

        return metadata.Handler;
    }

    /// <summary>
    ///     Retrieves a metadata from the endpoint descriptor.
    /// </summary>
    /// <typeparam name="TMetadata">
    ///     Type of the metadata.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/>.
    /// </param>
    /// <returns>
    ///     A collection of metadata.
    /// </returns>
    public static IEnumerable<TMetadata> GetAdditionalMetadata<TMetadata>(this IEndpointDescriptor self) {
        return self.AdditionalMetadata
                   .Where(metadata => metadata is TMetadata)
                   .Cast<TMetadata>();
    }
}
