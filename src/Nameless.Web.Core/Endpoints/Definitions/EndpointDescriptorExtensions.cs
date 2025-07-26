using System.Reflection;

namespace Nameless.Web.Endpoints.Definitions;

/// <summary>
///     Extension methods for <see cref="IEndpointDescriptor"/>.
/// </summary>
public static class EndpointDescriptorExtensions {
    /// <summary>
    ///     Ensures that the endpoint descriptor has a valid name.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/> instance.
    /// </param>
    /// <returns>
    ///     The name of the endpoint, ensuring it is valid.
    /// </returns>
    public static string EnsureName(this IEndpointDescriptor self) {
        return string.IsNullOrWhiteSpace(self.Name)
            ? $"{self.EndpointType.Name}_v{self.Version.Number}"
            : self.Name;
    }

    /// <summary>
    ///     Retrieves the endpoint handler method from the endpoint type.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IEndpointDescriptor"/> instance.
    /// </param>
    /// <returns>
    ///     The <see cref="MethodInfo"/> representing the endpoint handler
    ///     method.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if the endpoint handler method is not found.
    /// </exception>
    public static MethodInfo GetEndpointHandler(this IEndpointDescriptor self) {
        return self.EndpointType
                   .GetMethod(nameof(IEndpoint<object>.ExecuteAsync), BindingFlags.Instance | BindingFlags.Public)
               ?? throw new InvalidOperationException("Endpoint handler not found.");
    }
}
