namespace Nameless.Lucene.Mapping;

public abstract record PropertyDescriptor {
    public required string Name { get; init; }

    public required Type Type { get; init; }

    public PropertyOptions Options { get; init; }
}

public record PropertyDescriptor<TEntity> : PropertyDescriptor
    where TEntity : class {
    public required Func<TEntity, object?> Getter { get; init; }

    public required Action<TEntity, object?> Setter { get; init; }
}