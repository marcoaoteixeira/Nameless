using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents;

namespace Nameless.Lucene.Repository.Mappings;

public class Mapper : IMapper {
    private readonly IEntityDescriptorProvider _provider;

    public Mapper(IEntityDescriptorProvider provider) {
        _provider = provider;
    }

    Document IMapper.Map<TEntity>(TEntity entity) where TEntity : class {
        var descriptor = _provider.GetDescriptor<TEntity>();
        var result = new Document();

        foreach (var property in descriptor.Properties.Cast<PropertyDescriptor<TEntity>>()) {
            if (property.TryCreateField(entity, out var output)) {
                result.Add(output);
            }
        }

        return result;
    }

    TEntity IMapper.Map<TEntity>(Document document) where TEntity : class {
        var descriptor = _provider.GetDescriptor<TEntity>();
        var result = Activator.CreateInstance<TEntity>();

        foreach (var field in document) {
            var property = descriptor.Properties
                                     .Cast<PropertyDescriptor<TEntity>>()
                                     .SingleOrDefault(prop => prop.Name == field.Name);
            if (property is null) { continue; }

            if (field.TryGetValue(property, out var output)) {
                property.Setter(result, output);
            }
        }

        return result;
    }
    
    bool IMapper.TryGetID<TEntity>([NotNullWhen(returnValue: true)] out PropertyDescriptor<TEntity>? output)
        where TEntity : class {
        output = _provider.GetDescriptor<TEntity>()
                              .Properties
                              .SingleOrDefault(prop => prop.IsID) as PropertyDescriptor<TEntity>;

        return output is not null;
    }
}