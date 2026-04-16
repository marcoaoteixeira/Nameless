using System.Reflection;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Represents the endpoint information after being constructed.
/// </summary>
/// <param name="Target">The endpoint instance.</param>
/// <param name="Handler">The endpoint handler method.</param>
public record EndpointCall(IEndpoint Target, MethodInfo Handler);