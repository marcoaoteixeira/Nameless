using Microsoft.EntityFrameworkCore;

namespace Nameless.Testing.Tools.Mockers;

public class DbSetMocker<TEntity> : Mocker<DbSet<TEntity>>
    where TEntity : class {
    public DbSetMocker<TEntity> WithQueryable(IQueryable<TEntity> data) {
        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.Provider)
            .Returns(data.Provider);

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.Expression)
            .Returns(data.Expression);

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.ElementType)
            .Returns(data.ElementType);

        MockInstance
            .As<IQueryable<TEntity>>()
            .Setup(mock => mock.GetEnumerator())
            .Returns(data.GetEnumerator);

        return this;
    }
}