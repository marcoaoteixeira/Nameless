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
}
