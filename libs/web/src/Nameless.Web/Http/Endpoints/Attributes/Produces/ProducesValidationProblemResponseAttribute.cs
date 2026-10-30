using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a <c>ValidationProblemDetails</c> response for an endpoint,
///     typically used to document a <c>400 Bad Request</c> caused by
///     invalid input.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesValidationProblemResponseAttribute"/>
///     instances may be applied to the same endpoint class. The source
///     generator emits a corresponding
///     <c>ProducesValidationProblem(statusCode, contentType)</c> call on
///     the route handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ProducesValidationProblemResponseAttribute : ProducesResponseAttribute<ProblemDetails>;