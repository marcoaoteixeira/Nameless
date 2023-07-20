namespace Nameless.Collections.Generic {
    /// <summary>
    /// Defines methods and properties that will represent a page of items.
    /// </summary>
    /// <typeparam name="T">Type of the page.</typeparam>
    public interface IPage<T> : IEnumerable<T> {
        #region Properties

        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Gets the number of the page (<see cref="Index"/> + 1).
        /// </summary>
        int Number { get; }
        /// <summary>
        /// Gets the expected size of the page.
        /// </summary>
        int Size { get; }
        /// <summary>
        /// Gets how many items there are in this page.
        /// </summary>
        int Length { get; }
        /// <summary>
        /// Gets how many pages are possible to paginate.
        /// </summary>
        int PageCount { get; }
        /// <summary>
        /// Gets the total number of items related to the output collection.
        /// </summary>
        int Total { get; }
        /// <summary>
        /// Whether if there is a next page.
        /// </summary>
        bool HasNext { get; }
        /// <summary>
        /// Whether if there is a previous page.
        /// </summary>
        bool HasPrevious { get; }

        #endregion
    }
}
