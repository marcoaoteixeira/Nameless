namespace Nameless.Web.Generators.Models;

public record EndpointHandlerParameterModel {
    public required string Name { get; init; }
    public required string Type { get; init; }
    public bool IsCancellationToken { get; init; }
    public ParameterBindingKind BindingKind { get; init; }
    public string? BindingName { get; init; }
}