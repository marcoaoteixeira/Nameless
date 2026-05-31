namespace Nameless.Web.Http.Endpoints.Generator.Models;

public record ClassModel {
    public string Namespace { get; init; } = string.Empty;
    public required string Name { get; init; }
    public required string AccessorModifier { get; init; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";
}