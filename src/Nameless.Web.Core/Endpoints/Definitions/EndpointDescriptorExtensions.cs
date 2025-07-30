using System.Reflection;
using Microsoft.AspNetCore.Http;

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
    public static string GetName(this IEndpointDescriptor self) {
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
    ///     <see cref="IEndpointDescriptor.Action"/> inside
    ///     <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <see cref="IEndpointDescriptor.Action"/> inside
    ///     <paramref name="self"/> is empty or only whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if method not found in endpoint type or
    ///     return type for the method is not assignable
    ///     from <see cref="Task{TResult}"/> where TResult
    ///     is <see cref="IResult"/>.
    /// </exception>
    internal static MethodInfo GetAction(this IEndpointDescriptor self) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(self.EndpointType, message: $"Endpoint descriptor is missing '{nameof(IEndpointDescriptor.EndpointType)}' property.");
        Prevent.Argument.NullOrWhiteSpace(self.Action, message: $"Endpoint descriptor is missing '{nameof(IEndpointDescriptor.Action)}' property.");

        var handler = self.EndpointType
                          .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          .SingleOrDefault(method => method.Name == self.Action);

        if (handler is null) {
            throw new InvalidOperationException($"Action '{self.Action}' not found in endpoint '{self.EndpointType.Name}'.");
        }

        var returnType = typeof(Task<IResult>);
        if (!returnType.IsAssignableFrom(handler.ReturnType)) {
            throw new InvalidOperationException($"Action '{self.Action}' must return type '{returnType.GetPrettyName()}'.");
        }

        return handler;
    }

    internal static void EnsureAction(this IEndpointDescriptor self) {
        // This will ensure that we have a valid
        // endpoint handler to work with.
        _ = self.GetAction();
    }
}
