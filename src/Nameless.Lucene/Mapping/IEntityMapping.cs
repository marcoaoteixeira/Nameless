namespace Nameless.Lucene.Mapping;

public interface IEntityMapping {
    Type Type { get; }

    IReadOnlyCollection<PropertyDescriptor> Entries { get; }
}