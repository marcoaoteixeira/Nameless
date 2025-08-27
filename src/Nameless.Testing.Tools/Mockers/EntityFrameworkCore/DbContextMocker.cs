using Microsoft.EntityFrameworkCore;
using Moq;

namespace Nameless.Testing.Tools.Mockers.EntityFrameworkCore;

public class DbContextMocker : Mocker<DbContext> {
    public DbContextMocker WithSet<TEntity>(DbSet<TEntity>? returnValue = null)
        where TEntity : class {
        returnValue ??= new DbSetMocker<TEntity>().With(Enumerable.Empty<TEntity>().AsQueryable())
                                                  .Build();

        MockInstance.Setup(mock => mock.Set<TEntity>())
                    .Returns(returnValue);

        return this;
    }

    public DbContextMocker WithSet<TEntity>(IEnumerable<TEntity>? returnValue = null)
        where TEntity : class {
        var dbSet = new DbSetMocker<TEntity>().With((returnValue ?? []).AsQueryable())
                                              .Build();

        MockInstance.Setup(mock => mock.Set<TEntity>())
                    .Returns(dbSet);

        return this;
    }

    public DbContextMocker WithSaveChangesAsync(int returnValue = 1) {
        MockInstance.Setup(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(returnValue));

        return this;
    }
}