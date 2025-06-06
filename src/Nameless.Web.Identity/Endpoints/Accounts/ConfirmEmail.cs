﻿using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Nameless.Web.Endpoints;
using Nameless.Web.Filters;
using Nameless.Web.Identity.Endpoints.Accounts.Requests;
using Nameless.Web.Identity.Endpoints.Accounts.Responses;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Nameless.Web.Identity.Endpoints.Accounts;

public sealed class ConfirmEmail : EndpointBase {
    private readonly IOptions<IdentityOptions> _options;
    private readonly UserManager<User> _userManager;

    public override string HttpMethod
        => HttpMethods.POST;

    public override string RoutePattern
        => $"{_options.Value.BaseUrl}{Constants.Endpoints.CONFIRM_EMAIL}";

    public ConfirmEmail(UserManager<User> signInManager, IOptions<IdentityOptions> options) {
        _userManager = Prevent.Argument.Null(signInManager);
        _options = Prevent.Argument.Null(options);
    }

    public override void Configure(IEndpointConfiguration configuration) {
        configuration.WithOpenApi()
               .WithGroupName("Identity/Accounts")
               .WithName("identity_accounts_confirm_email")
               .WithDisplayName("Confirm E-mail")
               .WithDescription("Confirms the user e-mail address.")
               .AddEndpointFilter<ValidateEndpointFilter>()
               .AllowAnonymous()
               .Produces<ConfirmEmailResponse>()
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .ProducesValidationProblem();
    }

    public override Delegate CreateDelegate() {
        return HandleAsync;
    }

    public async Task<IResult> HandleAsync([FromBody] ConfirmEmailRequest input, CancellationToken cancellation) {
        var currentUser = await _userManager.FindByIdAsync(input.UserId);

        if (currentUser is null) {
            return HttpResults.Problem(
                string.Format(Constants.Messages.ConfirmEmail.UNABLE_LOAD_USER_MESSAGE, input.UserId),
                statusCode: StatusCodes.Status404NotFound,
                title: Constants.Messages.ConfirmEmail.UNABLE_LOAD_USER_TITLE);
        }

        var currentCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input.Code));
        var confirmEmailResult = await _userManager.ConfirmEmailAsync(currentUser, currentCode);

        return confirmEmailResult.Succeeded
            ? HttpResults.Ok(new ConfirmEmailResponse {
                Message = Constants.Messages.ConfirmEmail.CONFIRM_EMAIL_SUCCEEDED
            })
            : HttpResults.Problem(Constants.Messages.ConfirmEmail.CONFIRM_EMAIL_FAILED_MESSAGE,
                statusCode: StatusCodes.Status400BadRequest,
                title: Constants.Messages.ConfirmEmail.CONFIRM_EMAIL_FAILED_TITLE);
    }
}