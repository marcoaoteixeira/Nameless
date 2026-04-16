namespace Nameless.Lucene.Repository.Mappings;

public record PropertyDescriptor<TDocument> : PropertyDescriptor
    where TDocument : class {
    public required Func<TDocument, object?> Getter { get; init; }

    public required Action<TDocument, object?> Setter { get; init; }
}
