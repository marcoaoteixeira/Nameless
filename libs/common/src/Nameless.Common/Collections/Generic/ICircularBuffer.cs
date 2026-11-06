namespace Nameless.Collections.Generic;

/// <summary>
///     Circular buffer contract.
///     See <a href="https://en.wikipedia.org/wiki/Circular_buffer">Circular Buffer on Wikipedia</a>.
/// </summary>
/// <typeparam name="TElement">Type of the elements of the buffer</typeparam>
public interface ICircularBuffer<TElement> : IEnumerable<TElement> {
    /// <summary>
    ///     Gets an element by its index.
    /// </summary>
    /// <param name="index">The element index.</param>
    /// <returns>The element.</returns>
    TElement this[int index] { get; }

    /// <summary>
    ///     Gets the total number of elements.
    /// </summary>
    int Count { get; }

    /// <summary>
    ///     Gets the capacity of the buffer.
    /// </summary>
    int Capacity { get; }

    /// <summary>
    ///     Adds a new element to the buffer.
    /// </summary>
    /// <param name="element">The element.</param>
    void Add(TElement element);

    /// <summary>
    ///     Clears the buffer.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Checks if the element is part of the buffer.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    ///     <see langword="true"/> if the buffer contains the element, otherwise; <see langword="false"/>.
    /// </returns>
    bool Contains(TElement element);

    /// <summary>
    ///     Retrieves the index of the element inside the buffer.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The element index.</returns>
    int IndexOf(TElement element);

    /// <summary>
    ///     Copies this buffer to an array.
    /// </summary>
    /// <param name="array">The array that will receive this buffer elements.</param>
    /// <param name="startIndex">The index from where the copy will start.</param>
    void CopyTo(TElement[] array, int startIndex);

    /// <summary>
    ///     Creates a copy of the buffer as an array.
    /// </summary>
    /// <returns>An array of elements.</returns>
    TElement[] ToArray();
}