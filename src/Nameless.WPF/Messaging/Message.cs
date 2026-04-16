namespace Nameless.WPF.Messaging;

public abstract record Message {
    public string Title { get; init; } = string.Empty;
    
    public string Content { get; init; } = string.Empty;
    
    public MessageType Type { get; init; }

    public Dictionary<string, object?> Metadata { get; init; } = [];
}