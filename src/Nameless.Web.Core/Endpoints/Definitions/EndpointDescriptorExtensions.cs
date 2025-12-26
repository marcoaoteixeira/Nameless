using System.Reflection;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Extension methods for <see cref="IEndpointDescriptor"/>.
/// </summary>
public static class EndpointDescriptorExtensions {
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/> instance.
    /// </param>
    extension(IEndpointDescriptor self) {
        /// <summary>
        ///     Retrieves the name for the endpoint or the default value for
        ///     the name. The default value is a composition between the endpoint
        ///     type name and the version number.
        /// </summary>
        /// <returns>
        ///     The name of the endpoint, or the default value.
        /// </returns>
        public string GetNameOrDefault() {
            return string.IsNullOrWhiteSpace(self.Name)
                ? $"{self.EndpointType.Name}_v{self.Version.Number}"
                : self.Name;
        }

        /// <summary>
        ///     Retrieves the endpoint action method.
        /// </summary>
        /// <returns>
        ///     The action for the endpoint.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="self"/> or;
        ///     if <see cref="IEndpointDescriptor.EndpointType"/> or
        ///     <see cref="IEndpointDescriptor.ActionName"/> inside
        ///     <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     when endpoint handler metadata is not found.
        /// </exception>
        public MethodInfo GetEndpointHandler() {
            var metadata = self.GetAdditionalMetadata<EndpointHandlerMetadata>()
                               .SingleOrDefault();
            if (metadata is null) {
                throw new InvalidOperationException(message: "Endpoint descriptor missing handler metadata.");
            }

            return metadata.Handler;
        }

        /// <summary>
        ///     Retrieves a metadata from the endpoint descriptor.
        /// </summary>
        /// <typeparam name="TMetadata">
        ///     Type of the metadata.
        /// </typeparam>
        /// <returns>
        ///     A collection of metadata.
        /// </returns>
        public IEnumerable<TMetadata> GetAdditionalMetadata<TMetadata>() {
            return self.AdditionalMetadata
                       .Where(metadata => metadata is TMetadata)
                       .Cast<TMetadata>();
        }
    }
}