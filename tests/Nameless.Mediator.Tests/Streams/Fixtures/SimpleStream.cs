namespace Nameless.Mediator.Streams.Fixtures;

public class SimpleStream : IStream<string> {
    public string[] Messages { get; set; }
}