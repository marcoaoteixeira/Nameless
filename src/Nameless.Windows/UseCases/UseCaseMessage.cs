using Nameless.Windows.Messaging;

namespace Nameless.Windows.UseCases;

public record UseCaseMessage : Message {
    public string UseCase { get; init; } = string.Empty;
}