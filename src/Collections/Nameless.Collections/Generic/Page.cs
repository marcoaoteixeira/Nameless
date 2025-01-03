﻿using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Represents a page of items.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public sealed class Page<TItem> : IEnumerable<TItem> {
    public const int DEFAULT_NUMBER = 1;
    public const int DEFAULT_SIZE = 10;

    private readonly TItem[] _items;

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
    public int Count => _items.Length;

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
    /// <remarks>
    /// If the values provided to <paramref name="number"/> or <paramref name="size"/>
    /// were less than <c>1</c>, then their values will be set to their mentioned defaults.
    /// Values <c>1</c> and <c>10</c> respectively.
    /// </remarks>
    public Page(TItem[] items, int number = DEFAULT_NUMBER, int size = DEFAULT_SIZE) {
        _items = Prevent.Argument.Null(items);

        Number = number >= 1 ? number : DEFAULT_NUMBER;
        Size = size >= 1 ? size : DEFAULT_SIZE;
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator()
        => (IEnumerator<TItem>)_items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => _items.GetEnumerator();
}