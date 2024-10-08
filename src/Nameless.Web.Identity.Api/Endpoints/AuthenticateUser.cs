﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Web.Endpoints;
using Nameless.Web.Identity.Api.Inputs;
using Nameless.Web.Identity.Api.Outputs;
using Nameless.Web.Identity.Api.Requests;

namespace Nameless.Web.Identity.Api.Endpoints;

public sealed class AuthenticateUser : EndpointBase {
    private readonly IdentityApiOptions _options;

    public override string HttpMethod => Root.HttpMethods.POST;

    public override string RoutePattern => $"{_options.BaseUrl}/auth";

    public override bool UseValidationFilter => false;

    public AuthenticateUser(IdentityApiOptions options) {
        _options = Prevent.Argument.Null(options);
    }

    public override OpenApiMetadata GetOpenApiMetadata()
        => new () {
            Name = "Authentication",
            Description = Constants.Endpoints
                                   .Descriptions
                                   .AUTHENTICATE_USER,
            Summary = Constants.Endpoints
                               .Summaries
                               .AUTHENTICATE_USER,
            GroupName = Constants.Endpoints
                                 .Groups
                                 .AUTH,
            Produces = [
                Produces.Result<AuthenticateUserOutput>(),
                Produces.ValidationProblem()
            ]
        };

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