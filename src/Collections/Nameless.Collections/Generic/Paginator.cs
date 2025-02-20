using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Implementation of <see cref="IPaginator{TItem}"/>
/// </summary>
/// <typeparam name="TItem">The type of the page's items.</typeparam>
public sealed class Paginator<TItem> : IPaginator<TItem>, IEnumerable<IPage<TItem>> {
    private readonly IQueryable<TItem> _query;

    /// <inheritdoc />
    public int Total => _query.Count();

    /// <inheritdoc />
    public int PageSize { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Paginator{TItem}"/>.
    /// </summary>
    /// <param name="query">The query that will provide the items.</param>
    /// <param name="pageSize">The page size.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="query"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// If the value provided for <paramref name="pageSize"/> is lower than <c>1</c>,
    /// then the value will be set to <see cref="Page{TItem}.DEFAULT_SIZE"/>.
    /// </remarks>
    public Paginator(IQueryable<TItem> query, int pageSize = Page<TItem>.DEFAULT_SIZE) {
        _query = Prevent.Argument.Null(query);

        PageSize = pageSize >= 1 ? pageSize : Page<TItem>.DEFAULT_SIZE;
    }

    /// <inheritdoc />
    public IEnumerable<IPage<TItem>> GetPages() {
        var totalPages = Total / PageSize;
        if (Total % PageSize != 0) {
            ++totalPages;
        }

        for (var pageIndex = 0; pageIndex < totalPages; pageIndex++) {
            var items = _query.Skip(pageIndex * PageSize)
                              .Take(PageSize)
                              .ToArray();

            yield return new Page<TItem>(items: items,
                                         number: pageIndex + 1,
                                         size: PageSize);
        }
    }

    /// <inheritdoc />
    public IEnumerator<IPage<TItem>> GetEnumerator()
        => GetPages().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
