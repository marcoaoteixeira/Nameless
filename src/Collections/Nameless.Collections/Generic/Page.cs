using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Represents a page of items.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public sealed class Page<TItem> : IPage<TItem>, IEnumerable<TItem> {
    public const int DEFAULT_NUMBER = 1;
    public const int DEFAULT_SIZE = 10;

    /// <summary>
    /// Gets the page number.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the page total number of items.
    /// </summary>
    public int Count => Items.Length;

    /// <summary>
    /// Gets the page items.
    /// </summary>
    public TItem[] Items { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Page{TItem}"/>.
    /// </summary>
    /// <param name="items">The items of this page.</param>
    /// <param name="number">
    /// The page number. If not provided, the default value is <see cref="DEFAULT_NUMBER"/>.
    /// </param>
    /// <param name="size">
    /// The page size. If not provided, the default value is <see cref="DEFAULT_SIZE"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="items"/> is <c>null</c>.
    /// </exception>
    /// /// <remarks>
    /// If the value provided for <paramref name="size"/> is lower than <c>1</c>,
    /// then the value will be set to <see cref="DEFAULT_SIZE"/>.
    /// </remarks>
    public Page(TItem[] items, int number = DEFAULT_NUMBER, int size = DEFAULT_SIZE) {
        Items = Prevent.Argument.Null(items);

        Number = number >= 1 ? number : DEFAULT_NUMBER;
        Size = size >= 1 ? size : DEFAULT_SIZE;
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator()
        => (IEnumerator<TItem>)Items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => Items.GetEnumerator();
}