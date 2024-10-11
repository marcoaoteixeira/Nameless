using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Endpoints;
using Nameless.Web.Identity.Api.Inputs;
using Nameless.Web.Identity.Api.Outputs;
using Nameless.Web.Identity.Api.Requests;

namespace Nameless.Web.Identity.Api.Endpoints;

public sealed class GetUser : MinimalEndpointBase {
    private readonly IdentityApiOptions _options;

    public override string HttpMethod => HttpMethods.GET;

    public override string RoutePattern => $"{_options.BaseUrl}/account/user";

    public GetUser(IdentityApiOptions options) {
        _options = Prevent.Argument.Null(options);
    }

    public override void Configure(IMinimalEndpointBuilder builder)
        => builder.WithOpenApi()
                  .WithName("Get user")
                  .WithDescription(Constants.Endpoints.Descriptions.GET_USER)
                  .WithSummary(Constants.Endpoints.Summaries.GET_USER)
                  .WithGroupName(Constants.Endpoints.Groups.USERS)
                  .WithApiVersionSet()
                  .HasApiVersion(1)
                  .Produces<AuthenticateUserOutput>()
                  .ProducesValidationProblem();

    public override Delegate CreateDelegate() => HandleAsync;

    private static async Task<IResult> HandleAsync([FromBody] AuthenticateUserInput input,
                                                   [FromServices] IMediator mediator,
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