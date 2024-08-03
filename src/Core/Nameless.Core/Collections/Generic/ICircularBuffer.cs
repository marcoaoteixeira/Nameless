namespace Nameless.Collections.Generic {
    /// <summary>
    /// Provides a contract to implement a circular buffer. See: https://en.wikipedia.org/wiki/Circular_buffer
    /// </summary>
    /// <typeparam name="T">Type of the circular buffer.</typeparam>
    public interface ICircularBuffer<T> : IEnumerable<T> {
        #region Properties

        T this[int index] { get; }

        int Count { get; }
        int Capacity { get; }

        #endregion

        #region Methods

        void Add(T item);
        void Clear();
        bool Contains(T item);
        int IndexOf(T item);
        void CopyTo(T[] array, int arrayIndex);
        T[] ToArray();

        #endregion
    }
}
