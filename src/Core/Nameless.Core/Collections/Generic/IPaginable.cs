namespace Nameless.Collections.Generic;

/// <summary>
/// Contract to enable paging through items.
/// </summary>
/// <typeparam name="T">Type of the page items.</typeparam>
public interface IPaginable<T> : IEnumerable<Page<T>> {
    int Total { get; }
    int PageSize { get; }
}