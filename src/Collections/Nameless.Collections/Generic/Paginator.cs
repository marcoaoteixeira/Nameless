using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Implementation of <see cref="IPaginator{TItem}"/>
/// </summary>
/// <typeparam name="TItem">The type of the page's items.</typeparam>
public sealed class Paginator<TItem> : IPaginator<TItem> {
    public const int DEFAULT_PAGE_SIZE = 10;

    private readonly IQueryable<TItem> _query;

    /// <inheritdoc />
    public int Total => _query.Count();

    /// <inheritdoc />
    public int PageSize { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Paginator{TItem}"/>.
    /// </summary>
    /// <param name="query">
    /// The query that will provide the items.
    /// </param>
    /// <param name="pageSize">
    /// The page size. Default value is <see cref="DEFAULT_PAGE_SIZE"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="query"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// If the value provided to <paramref name="pageSize"/> is lower than <c>1</c>,
    /// then the value will be set to <see cref="DEFAULT_PAGE_SIZE"/>.
    /// </remarks>
    public Paginator(IQueryable<TItem> query, int pageSize = DEFAULT_PAGE_SIZE) {
        _query = Prevent.Argument.Null(query);

        PageSize = pageSize >= 1 ? pageSize : DEFAULT_PAGE_SIZE;
    }

    public IEnumerator<Page<TItem>> GetEnumerator()
        => GetPages().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private IEnumerable<Page<TItem>> GetPages() {
        var pages = Total / PageSize;
        if (Total % PageSize != 0) {
            ++pages;
        }

        for (var pageIndex = 0; pageIndex < pages; pageIndex++) {
            var items = _query.Skip(pageIndex * PageSize)
                                  .Take(PageSize)
                                  .ToArray();

            yield return new Page<TItem>(items: items,
                                     number: pageIndex + 1,
                                     size: PageSize);
        }
    }
}
