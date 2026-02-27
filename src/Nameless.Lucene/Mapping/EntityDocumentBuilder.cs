using Lucene.Net.Documents;

namespace Nameless.Lucene.Mapping;

public class EntityDocumentBuilder {
    private readonly IReadOnlyCollection<IEntityMapping> _mappings;

    public EntityDocumentBuilder(IEnumerable<IEntityMapping> mappings) {
        _mappings = [.. mappings];
    }
    
    public TEntity Build<TEntity>(Document document) where TEntity : class, new() {
        var descriptors = GetDescriptors<TEntity>();
        var instance = new TEntity();

        foreach (var field in document) {
            var descriptor = descriptors.GetValueOrDefault(field.Name);
            
            if (descriptor is null) { continue; }

            if (IndexableFieldHelper.TryGetValue(descriptor, field, out var value)) {
                descriptor.Setter(instance, value);
            }
        }

        return instance;
    }

    public Document Build<TEntity>(TEntity instance) where TEntity : class {
        var result = new Document();

        foreach (var descriptor in GetDescriptors<TEntity>().Values) {
            var value = descriptor.Getter(instance);

            if (IndexableFieldHelper.TryCreate(descriptor, value, out var field)) {
                result.Add(field);
            }
        }

        return result;
    }

    private EntityMapping<TEntity> GetMapping<TEntity>() where TEntity : class {
        var mapping = _mappings.OfType<EntityMapping<TEntity>>()
                               .SingleOrDefault(item => item.Type == typeof(TEntity));

        return mapping ?? throw new InvalidOperationException("Missing mapping type.");
    }

    public IReadOnlyDictionary<string, PropertyDescriptor<TEntity>> GetDescriptors<TEntity>() where TEntity : class {
        var result = GetMapping<TEntity>().Entries.OfType<PropertyDescriptor<TEntity>>().ToDictionary(
            item => item.Name,
            item => item
        );

        if (!result.ContainsKey(Constants.DOCUMENT_ID_PROP)) {
            throw new InvalidOperationException($"Mapping for '{typeof(TEntity).GetPrettyName()}' is missing the ID property mapping.");
        }

        return result;
    }
}