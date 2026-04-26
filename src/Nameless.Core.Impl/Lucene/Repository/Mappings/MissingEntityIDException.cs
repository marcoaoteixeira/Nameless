namespace Nameless.Lucene.Repository.Mappings;

public class MissingEntityIDException : Exception {
    public MissingEntityIDException(Type type)
        : base ($"Entity type '{type.GetPrettyName()}' is missing its ID property descriptor.") { }
}
