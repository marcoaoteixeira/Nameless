using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Endpoints;
using Nameless.Web.Identity.Api.Inputs;
using Nameless.Web.Identity.Api.Outputs;
using Nameless.Web.Identity.Api.Requests;

namespace Nameless.Web.Identity.Api.Endpoints;

public sealed class GetUser : IEndpoint {
    private readonly IdentityApiOptions _options;

    public string HttpMethod => Root.HttpMethods.GET;

    public string RoutePattern => $"{_options.BaseUrl}/{Name}";

    public string Name => "users";

    public string Description => Constants.Endpoints
                                          .Descriptions
                                          .GET_USER;

    public string Summary => Constants.Endpoints
                                      .Summaries
                                      .GET_USER;

    public string GroupName => Constants.Endpoints
                                        .Groups
                                        .USERS;

    public string[] Tags => [];

    public AcceptMetadata[] Accepts => [];

    public int Version => 1;

    public bool Deprecated => false;

    public int MapToVersion => 0;

    public ProducesMetadata[] Produces => [
        new() { ResponseType = typeof(AuthenticateUserOutput) },
        new() { Type = ProducesResultType.ValidationProblems }
    ];

    public GetUser(IdentityApiOptions options) {
        _options = Prevent.Argument.Null(options);
    }

    public Delegate CreateDelegate() => async ([FromBody] AuthenticateUserInput input,
                                               [FromServices] IMediator mediator,
                                               CancellationToken cancellationToken) => {
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
    };
}