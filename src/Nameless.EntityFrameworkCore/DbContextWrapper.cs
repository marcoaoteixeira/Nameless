using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     A wrapper implementation for <see cref="DbContext"/> that implements
///     <see cref="IDbContext"/>.
/// </summary>
public class DbContextWrapper : IDbContext {
    private readonly DbContext _dbContext;

    /// <summary>
    ///     Initializes a new instance of <see cref="DbContextWrapper"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The current <see cref="DbContext"/>.
    /// </param>
    public DbContextWrapper(DbContext dbContext) {
        _dbContext = Guard.Against.Null(dbContext);
    }

    /// <inheritdoc />
    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class {
        return _dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    public DbSet<TEntity> Set<TEntity>(string name)
        where TEntity : class {
        return _dbContext.Set<TEntity>(name);
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}