using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository;

internal static class EntityDescriptorExtensions {
    extension<TEntity>(IEntityDescriptor<TEntity> self)
        where TEntity : class {
        internal bool HasID => self.Properties.Any(prop => prop.IsID);
    }
}
