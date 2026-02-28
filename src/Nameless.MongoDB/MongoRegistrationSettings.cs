using Nameless.Registration;

namespace Nameless.MongoDB;

public class MongoRegistrationSettings : AssemblyScanAware<MongoRegistrationSettings> {
    private readonly HashSet<Type> _documentMappings = [];

    public IReadOnlyCollection<Type> DocumentMappings => UseAssemblyScan
        ? DiscoverImplementationsFor<IDocumentMapping>()
        : _documentMappings;

    public MongoRegistrationSettings RegisterDocumentMapping<TDocumentMapping>()
        where TDocumentMapping : IDocumentMapping, new() {
        return RegisterDocumentMapping(typeof(TDocumentMapping));
    }

    public MongoRegistrationSettings RegisterDocumentMapping(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IDocumentMapping));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);
        Throws.When.HasNoParameterlessConstructor(type);

        _documentMappings.Add(type);

        return this;
    }
}
