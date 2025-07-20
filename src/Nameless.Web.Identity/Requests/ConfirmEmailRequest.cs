using Nameless.Validation;

namespace Nameless.Web.Identity.Requests;

[Validate]
public record ConfirmEmailRequest(string UserID, string Code);