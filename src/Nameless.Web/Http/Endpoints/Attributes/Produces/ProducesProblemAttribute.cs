using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a <c>ProblemDetails</c> error response for an endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesProblemAttribute"/> instances may
///     be applied to the same endpoint class to document several possible
///     error responses. The source generator emits a corresponding
///     <c>ProducesProblem(statusCode, contentType)</c> call on the route
///     handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ProducesProblemAttribute : ProducesAttribute<ProblemDetails> {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ProducesProblemAttribute"/> with the specified status
    ///     code.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>500</c>.
    /// </param>
    /// <param name="contentType">
    ///     The content type.
    /// </param>
    public ProducesProblemAttribute(int statusCode = 500, string contentType = Constants.ProblemContentType)
        : base(statusCode, contentType) { }
}
