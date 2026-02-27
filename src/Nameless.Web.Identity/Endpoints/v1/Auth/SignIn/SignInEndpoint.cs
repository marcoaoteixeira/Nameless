using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Mediator;
using Nameless.Validation;
using Nameless.Web.Identity.UseCases.Authentication.SignIn;
using Nameless.Web.MinimalEndpoints;
using Nameless.Web.MinimalEndpoints.Definitions;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Identity.Endpoints.v1.Auth.SignIn;

/// <summary>
///     The Sign-In endpoint.
/// </summary>
public class SignInEndpoint : IEndpoint {
    private readonly IMediator _mediator;
    private readonly IValidationService _validationService;

    public SignInEndpoint(
        IMediator mediator,
        IValidationService validationService) {
        _mediator = Throws.When.Null(mediator);
        _validationService = Throws.When.Null(validationService);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Describe() {
        var builder = EndpointDescriptorBuilder.Create<SignInEndpoint>();

        builder
            .Post(routePattern: "signin", nameof(ExecuteAsync))
            .AllowAnonymous()
            .WithDescription(description: "Authenticates a user using the provided information.")
            .WithDisplayName(displayName: "SignIn")
            .WithGroupName(groupName: "auth")
            .Produces<SignInOutput>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem();

        return builder.Build();
    }

    public async Task<IResult> ExecuteAsync([FromBody] SignInInput input, CancellationToken cancellationToken) {
        Throws.When.Null(input);

        var validationResult = await _validationService.ValidateAsync(input, cancellationToken);
        if (!validationResult.Success) {
            return ValidationProblem(validationResult.ToDictionary());
        }

        var request = new SignInRequest { Email = input.Email, Password = input.Password };
        var signInResponse = await _mediator.ExecuteAsync(request, cancellationToken);

        return signInResponse.Success
            ? Ok(signInResponse)
            : Unauthorized();
    }
}