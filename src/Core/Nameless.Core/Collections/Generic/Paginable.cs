using System.Collections;

namespace Nameless.Collections.Generic;

public sealed class Paginable<T> : IPaginable<T> {
    private readonly IQueryable<T> _queryable;

    public int Total => _queryable.Count();

    public int PageSize { get; }

    public Paginable(IQueryable<T> queryable, int pageSize) {
        _queryable = Prevent.Argument.Null(queryable);

        PageSize = Prevent.Argument.LowerOrEqual(pageSize, to: 0);
    }

    public IEnumerator<Page<T>> GetEnumerator()
        => GetPages().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private IEnumerable<Page<T>> GetPages() {
        var pages = Total / PageSize;
        if (Total % PageSize != 0) {
            ++pages;
        }

        for (var pageIndex = 0; pageIndex < pages; pageIndex++) {
            var items = _queryable.Skip(pageIndex * PageSize)
                                  .Take(PageSize)
                                  .ToArray();

            yield return new Page<T>(items: items,
                                     number: pageIndex + 1,
                                     size: PageSize);
        }
    }
}
