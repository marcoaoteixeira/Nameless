using Nameless.Collections.Generic;

namespace Nameless;

/// <summary>
/// <see cref="Page{T}"/> extension methods.
/// </summary>
public static class PageExtension {
    /// <summary>
    /// Creates a <see cref="Page{T}"/> from the <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="self">The <see cref="IQueryable{T}"/> that will provide the items to the page.</param>
    /// <param name="index">The page index. Default is 0 (zero).</param>
    /// <param name="size">The page desired size. Default is 10.</param>
    /// <returns>An instance of <see cref="Page{T}"/>.</returns>
    public static IPage<T> AsPage<T>(this IQueryable<T> self, int index = 0, int size = 10)
        => new Page<T>(self, index, size);
}