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

public class ConfirmEmailEndpoint : Endpoint<> {
    private readonly UserManager<User> _userManager;

    public ConfirmEmailEndpoint(UserManager<User> userManager) {
        _userManager = Prevent.Argument.Null(userManager);
    }

    public override void Configure(IEndpointDescriptor descriptor) {
        descriptor
            .Get("accounts/confirm-email", HandleAsync)
            .WithGroupName("identity")
            .AllowAnonymous()
            .WithFilter<ValidateRequestEndpointFilter>()
            .Produces<ConfirmEmailResponse>()
            .ProducesProblem(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesValidationProblem();
    }

    public async Task<IResult> HandleAsync([AsParameters] ConfirmEmailRequest request, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var user = await _userManager.FindByIdAsync(request.UserID);
        if (user is null) {
            return Problem("User not found", statusCode: StatusCodes.Status404NotFound);
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded) {
            return Ok(new ConfirmEmailResponse("Email confirmed successfully."));
        }

        return Problem(
            detail: string.Join(", ", result.Errors.Select(error => error.Description)),
            statusCode: StatusCodes.Status400BadRequest,
            title: "Confirm Email Error"
        );
    }
}