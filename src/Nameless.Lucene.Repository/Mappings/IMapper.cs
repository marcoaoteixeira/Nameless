using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents;

namespace Nameless.Lucene.Repository.Mappings;

public interface IMapper {
    Document Map<TEntity>(TEntity entity) where TEntity : class;

    TEntity Map<TEntity>(Document document) where TEntity : class;

    bool TryGetID<TEntity>([NotNullWhen(returnValue: true)] out PropertyDescriptor<TEntity>? output) where TEntity : class;
}