using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Filters;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Requests;
using Nameless.Web.Identity.Responses;

namespace Nameless.Web.Identity.Endpoints.Management;

public class ConfirmEmailChangeEndpoint : Endpoint {
    private readonly UserManager<User> _userManager;

    public ConfirmEmailChangeEndpoint(UserManager<User> userManager) {
        _userManager = Prevent.Argument.Null(userManager);
    }

    public override void Configure(IEndpointDescriptor descriptor) {
        descriptor
            .Get("accounts/confirm-email-change", HandleAsync)
            .WithGroupName("identity")
            .AllowAnonymous()
            .WithFilter<ValidateRequestEndpointFilter>()
            .Produces<ConfirmEmailChangeResponse>()
            .ProducesProblem(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();
    }

    public async Task<IResult> HandleAsync([AsParameters] ConfirmEmailChangeRequest request, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var user = await _userManager.FindByIdAsync(request.UserID);
        if (user is null) {
            return Problem(
                detail: "User not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        var changeEmailResult = await _userManager.ChangeEmailAsync(user, request.Email, code);
        if (!changeEmailResult.Succeeded) {
            return Problem(
                detail: changeEmailResult.Errors.ToErrorMessage(),
                statusCode: StatusCodes.Status400BadRequest,
                title: "Confirm Email Change Error"
            );
        }

        // In our system, email and username are one and the same, so when we
        // update the email we need to update the username.
        var setUserNameResult = await _userManager.SetUserNameAsync(user, request.Email);
        if (!setUserNameResult.Succeeded) {
            return Problem(
                detail: setUserNameResult.Errors.ToErrorMessage(),
                statusCode: StatusCodes.Status400BadRequest,
                title: "Confirm Email Change Error"
            );
        }

        return Ok(new ConfirmEmailChangeResponse("Email change confirmed successfully."));
    }
}