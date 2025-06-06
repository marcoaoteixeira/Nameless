namespace Nameless.Web.Identity;

public record IdentityOptions {
    public string BaseUrl { get; init; } = string.Empty;

    public UserIdentifier UserIdentifier { get; init; }
    public bool LockoutOnFailure { get; init; } = false;
}

public enum UserIdentifier {
    Email,

    UserName
}