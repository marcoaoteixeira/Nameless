using System.Collections;

namespace Nameless.Collections.Generic {
    /// <summary>
    /// Represents a page of enumerable items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Page<T> : IPage<T> {
        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets a empty page of the defined type <see cref="T"/>.
        /// </summary>
        public static Page<T> Empty => new([]);

        #endregion

        #region Private Properties

        private T[] Items { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Page{T}"/>.
        /// </summary>
        /// <param name="items">The <see cref="IEnumerable{T}"/> that will provide the items to this page.</param>
        /// <param name="index">The page index. Default is 0 (zero).</param>
        /// <param name="size">The page desired size. Default is 10.</param>
        public Page(IEnumerable<T> items, int index = 0, int size = 10)
            : this((items ?? []).AsQueryable(), index, size) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Page{T}"/>.
        /// </summary>
        /// <param name="items">The <see cref="IQueryable{T}"/> that will provide the items to this page.</param>
        /// <param name="index">The page index. Default is 0 (zero).</param>
        /// <param name="size">The page desired size. Default is 10.</param>
        public Page(IQueryable<T> items, int index = 0, int size = 10) {
            Guard.Against.Null(items, nameof(items));

            index = index >= 0 ? index : 0;
            size = size > 0 ? size : 10;

            Index = index;
            Size = size;
            Total = items.Count();
            Items = [.. items.Skip(index * size).Take(size)];
        }

        #endregion

        #region IPage<T> Members

        public int Index { get; }

        public int Number => Index + 1;

        public int Size { get; }
        public int Length => Items.Length;

        public int PageCount => (int)Math.Ceiling(Total / (decimal)Size);

        public int Total { get; }

        public bool HasNext => Number < PageCount;

        public bool HasPrevious => Number > 1;

        #endregion

        #region IEnumerable<T> Members

        /// <inheritdocs />
        public IEnumerator<T> GetEnumerator()
            => Items.AsEnumerable().GetEnumerator();

        /// <inheritdocs />
        IEnumerator IEnumerable.GetEnumerator()
            => Items.GetEnumerator();

        #endregion
    }

    /// <summary>
    /// <see cref="Page{T}"/> extension methods.
    /// </summary>
    public static class PageExtension {
        #region Public Static Methods

        /// <summary>
        /// Creates a <see cref="Page{T}"/> from the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="self">The <see cref="IEnumerable{T}"/> that will provide the items to the page.</param>
        /// <param name="index">The page index. Default is 0 (zero).</param>
        /// <param name="size">The page desired size. Default is 10.</param>
        /// <returns>An instance of <see cref="Page{T}"/>.</returns>
        public static IPage<T> AsPage<T>(this IEnumerable<T> self, int index = 0, int size = 10)
            => new Page<T>(self, index, size);

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

        #endregion
    }
}
