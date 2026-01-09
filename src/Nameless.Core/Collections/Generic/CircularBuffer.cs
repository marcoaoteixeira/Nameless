using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
///     Default implementation of <see cref="ICircularBuffer{TElement}" />.
/// </summary>
/// <typeparam name="TElement">Type of the element.</typeparam>
public class CircularBuffer<TElement> : ICircularBuffer<TElement> {
    private readonly TElement[] _buffer;

    private int _end;
    private int _start;

    private bool IsEmpty => Count == 0;
    private bool IsFull => Count == Capacity;

    /// <summary>
    ///     Initializes a new instance of <see cref="CircularBuffer{T}" />.
    /// </summary>
    /// <param name="capacity">
    ///     The capacity of this circular buffer instance.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="capacity" /> is less than <c>1</c>.
    /// </exception>
    public CircularBuffer(int capacity)
        : this(capacity, []) {
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="CircularBuffer{T}" />.
    /// </summary>
    /// <param name="capacity">
    ///     The capacity of this circular buffer instance.
    /// </param>
    /// <param name="items">The startup items.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if <paramref name="capacity" /> is less than 1. Or
    ///     if length of <paramref name="items" /> is greater than
    ///     <paramref name="capacity" />.
    /// </exception>
    public CircularBuffer(int capacity, TElement[] items) {
        Guard.Against.LowerThan(capacity, compare: 1);
        Guard.Against.GreaterThan(
            items.Length,
            paramName: nameof(items),
            compare: capacity,
            message:
            $"Too many items. Maximum number of items must be less or equal to {nameof(capacity)} => {capacity}"
        );

        // let's create a copy of the current buffer.
        _buffer = new TElement[capacity];

        // copy all items to our inner buffer.
        if (items.Length > 0) {
            Array.Copy(
                items,
                _buffer,
                items.Length
            );
            Count = items.Length;
        }

        _start = 0;

        // if our total count is different from the
        // capacity request, set the end to the
        // Count (any new item will be added from here)
        // Otherwise; set the end and the start, this
        // will override the items as we add new ones.
        _end = Count != capacity ? Count : 0;
    }

    /// <inheritdoc />
    public TElement this[int index] {
        get => _buffer[GetIndex(index)];
        set => _buffer[GetIndex(index)] = value;
    }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <inheritdoc />
    public int Capacity => _buffer.Length;

    /// <inheritdoc />
    public void Add(TElement element) {
        _buffer[_end] = element;

        Increment(ref _end);

        if (IsFull) { _start = _end; }
        else { ++Count; }
    }

    /// <inheritdoc />
    public void Clear() {
        Count = _start = _end = 0;
    }

    /// <inheritdoc />
    public bool Contains(TElement element) {
        return IndexOf(element) > -1;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     <see cref="IndexOf" /> will compare if there is an element
    ///     inside the buffer that is equal to the element specified using
    ///     <see cref="object.Equals(object?, object?)" />.
    /// </remarks>
    public int IndexOf(TElement element) {
        for (var index = 0; index < Count; index++) {
            if (Equals(this[index], element)) {
                return index;
            }
        }

        return -1;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="array" /> does not have enough positions
    ///     to copy all elements from circular buffer.
    /// </exception>
    public void CopyTo(TElement[] array, int startIndex) {
        if (array.Length - startIndex < Count) {
            throw new InvalidOperationException(message: "Array does not contain enough space for items");
        }

        for (var index = 0; index < Count; ++index) {
            array[index + startIndex] = this[index];
        }
    }

    /// <inheritdoc />
    public TElement[] ToArray() {
        if (IsEmpty) { return []; }

        var array = new TElement[Count];
        for (var index = 0; index < Count; ++index) {
            array[index] = this[index];
        }

        return array;
    }

    /// <inheritdoc />
    public IEnumerator<TElement> GetEnumerator() {
        for (var index = 0; index < Count; index++) {
            yield return _buffer[index];
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private int GetIndex(int index) {
        if (IsEmpty) {
            throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
        }

        if (index >= Count) {
            throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}");
        }

        return (_start + index) % Capacity;
    }

    private void Increment(ref int index) {
        if (++index < Capacity) { return; }

        index = 0;
    }
}