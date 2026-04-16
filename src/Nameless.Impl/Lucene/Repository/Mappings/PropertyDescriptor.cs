namespace Nameless.Lucene.Repository.Mappings;

public abstract record PropertyDescriptor {
    public required string Name { get; init; }

    public required Type Type { get; init; }

    public bool IsID { get; init; }

    public PropertyOptions Options { get; init; }
}

public record PropertyDescriptor<TDocument> : PropertyDescriptor
    where TDocument : class {
    public required Func<TDocument, object?> Getter { get; init; }

    public required Action<TDocument, object?> Setter { get; init; }
}
