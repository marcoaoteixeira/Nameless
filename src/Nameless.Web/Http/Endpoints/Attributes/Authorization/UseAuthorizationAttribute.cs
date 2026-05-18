namespace Nameless.Web.Http.Endpoints.Attributes.Authorization;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class UseAuthorizationAttribute : Attribute {
    public string? PolicyName { get; init; }

    public string? Roles { get; init; }

    public string? AuthenticationSchemes { get; init; }
}