using Nameless.Validation;

namespace Nameless.Web.Identity.Requests;

[Validate]
public record ConfirmEmailChangeRequest(string UserID, string Email, string Code);