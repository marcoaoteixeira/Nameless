using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nameless.Web.Api;
using Nameless.Web.Identity.Api.Inputs;
using Nameless.Web.Identity.Api.Outputs;
using Nameless.Web.Identity.Api.Requests;

namespace Nameless.Web.Identity.Api.Endpoints;

public sealed class AuthenticateUser : IEndpoint {
    private readonly IdentityApiOptions _options;

    public string Name => "auth";

    public string Summary => Constants.Endpoints
                                      .Summaries
                                      .AUTHENTICATE_USER;

    public string Description => Constants.Endpoints
                                          .Descriptions
                                          .AUTHENTICATE_USER;

    public string Group => Constants.Endpoints
                                    .Groups
                                    .AUTH;

    public int Version => 1;

    public AuthenticateUser(IdentityApiOptions options) {
        _options = Prevent.Argument.Null(options);
    }

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder builder)
        => builder.MapPost($"{_options.BaseUrl}/{Name}", HandleAsync)
                  .Produces<AuthenticateUserOutput>()
                  .ProducesValidationProblem()
                  .ProducesProblem(StatusCodes.Status401Unauthorized)
                  .ProducesProblem(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> HandleAsync(
        [FromBody] AuthenticateUserInput input,
        IMediator mediator,
        CancellationToken cancellationToken) {
        var request = new AuthenticateUserRequest {
            UserName = input.Username,
            Password = input.Password
        };

        var response = await mediator.Send(request, cancellationToken);

        var output = new AuthenticateUserOutput {
            Token = response.Token,
            Error = response.Error,
            Succeeded = response.Succeeded()
        };

        return Results.Ok(output);
    }
}