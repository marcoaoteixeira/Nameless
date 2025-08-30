namespace Nameless.Mediator.Streams.Fixtures;

public record MessageStream(string[] Messages) : IStream<string>;