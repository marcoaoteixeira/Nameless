using Nameless.WPF.Messaging;

namespace Nameless.WPF.UseCases;

public record UseCaseMessage : Message {
    public string UseCase { get; init; } = string.Empty;
}