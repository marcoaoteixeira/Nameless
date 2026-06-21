using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Repository.Mappings;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Repository;

internal static class EntityExtensions {
    private const string MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR = "The number of clauses in the boolean query exceeds the maximum allowed (1024).";

    extension<TEntity>(IEnumerable<TEntity> self)
        where TEntity : class {
        internal Result<Query> CreateDeleteQuery(IMapper mapper) {
            var inner = self.ToArray();

            if (inner.Length > BooleanQuery.MaxClauseCount) {
                return Error.Failure(MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR);
            }

            if (!mapper.TryGetID<TEntity>(out var property)) {
                return Error.Failure($"Entity type '{typeof(TEntity).GetPrettyName()}' is missing its ID property descriptor.");
            }

            // create boolean clauses
            var clauses = inner.Select(entity => CreateBooleanClause(entity, property))
                               .Where(clause => clause is not null)
                               .Cast<BooleanClause>();

            var query = new BooleanQuery();

            query.Clauses.AddRange(clauses);

            return query;
        }

        internal DocumentCollection CreateDocumentCollection(IMapper mapper) {
            var collection = new DocumentCollection();

            foreach (var entity in self) {
                collection.Add(
                    document: mapper.Map(entity)
                );
            }

            return collection;
        }
    }

    private static BooleanClause? CreateBooleanClause<TEntity>(TEntity entity, PropertyDescriptor<TEntity> property)
        where TEntity : class {
        var id = property.Getter(entity);
        if (id is null) { return null; }

        var term = new Term(property.Name, id.ToString());
        var termQuery = new TermQuery(term);

        return new BooleanClause(termQuery, Occur.SHOULD);
    }
}
