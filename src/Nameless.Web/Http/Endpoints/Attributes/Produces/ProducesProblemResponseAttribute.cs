using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a <c>ProblemDetails</c> error response for an endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesProblemResponseAttribute"/> instances may
///     be applied to the same endpoint class to document several possible
///     error responses. The source generator emits a corresponding
///     <c>ProducesProblem(statusCode, contentType)</c> call on the route
///     handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ProducesProblemResponseAttribute : ProducesResponseAttribute<ProblemDetails>;