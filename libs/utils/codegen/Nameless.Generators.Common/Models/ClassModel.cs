namespace Nameless.Generators.Models;

public record ClassModel {
    public string Namespace { get; init; } = string.Empty;
    public required string Name { get; init; }
    public required string Accessibility { get; init; }
    public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";
}