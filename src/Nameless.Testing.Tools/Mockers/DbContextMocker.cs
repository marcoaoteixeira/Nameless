using Nameless.EntityFrameworkCore;

namespace Nameless.Testing.Tools.Mockers;

public class DbContextMocker : Mocker<IDbContext> {
    public DbContextMocker WithSet<TEntity>(IQueryable<TEntity> data)
        where TEntity : class {
        var dbSet = new DbSetMocker<TEntity>().WithQueryable(data)
                                              .Build();

        MockInstance
            .Setup(mock => mock.Set<TEntity>())
            .Returns(dbSet);

        return this;
    }
}