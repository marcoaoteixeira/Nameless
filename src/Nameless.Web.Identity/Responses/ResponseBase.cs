namespace Nameless.Web.Identity.Responses;

public record ErrorResponse {
    public required string Message { get; init; }
}
