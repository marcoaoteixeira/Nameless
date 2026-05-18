using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a <c>ValidationProblemDetails</c> response for an endpoint,
///     typically used to document a <c>400 Bad Request</c> caused by
///     invalid input.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesValidationProblemAttribute"/> instances
///     may be applied to the same endpoint class. The source generator
///     emits a corresponding
///     <c>ProducesValidationProblem(statusCode, contentType)</c> call on
///     the route handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ProducesValidationProblemAttribute : ProducesAttribute<ProblemDetails> {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ProducesValidationProblemAttribute"/> with the specified
    ///     status code.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>400</c>.
    /// </param>
    /// <param name="contentType">
    ///     The content type.
    /// </param>
    public ProducesValidationProblemAttribute(int statusCode = 400, string contentType = Constants.ProblemContentType)
        : base(statusCode, contentType) { }
}
