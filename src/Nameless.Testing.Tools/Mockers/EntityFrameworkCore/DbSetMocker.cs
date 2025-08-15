using Microsoft.EntityFrameworkCore;
using Moq;

namespace Nameless.Testing.Tools.Mockers.EntityFrameworkCore;

public class DbSetMocker<TEntity> : Mocker<DbSet<TEntity>>
    where TEntity : class {
    public DbSetMocker<TEntity> With(IQueryable<TEntity> returnValue) {
        MockInstance.As<IAsyncEnumerable<TEntity>>()
                    .Setup(mock => mock.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                    .Returns(() => CreateAsyncEnumerator(returnValue));

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.Provider)
            .Returns(() => CreateAsyncQueryProvider(returnValue));

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.Expression)
            .Returns(returnValue.Expression);

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.ElementType)
            .Returns(returnValue.ElementType);

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.GetEnumerator())
            .Returns(returnValue.GetEnumerator);

        return this;
    }

    private static InMemoryAsyncEnumerator<TEntity> CreateAsyncEnumerator(IQueryable<TEntity> queryable) {
        return new InMemoryAsyncEnumerator<TEntity>(queryable.GetEnumerator());
    }

    private static InMemoryAsyncQueryProvider<TEntity> CreateAsyncQueryProvider(IQueryable<TEntity> queryable) {
        return new InMemoryAsyncQueryProvider<TEntity>(queryable.Provider);
    }
}