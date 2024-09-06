using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Implementation of <see cref="ICircularBuffer{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the circular buffer.</typeparam>
public sealed class CircularBuffer<T> : ICircularBuffer<T> {
    private readonly T[] _buffer;

    private int _start;
    private int _end;

    private bool IsEmpty => Count == 0;
    private bool IsFull => Count == Capacity;

    /// <inheritdoc />
    public T this[int index] {
        get => _buffer[GetIndex(index)];
        set => _buffer[GetIndex(index)] = value;
    }

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <inheritdoc />
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Initializes a new instance of <see cref="CircularBuffer{T}"/>
    /// </summary>
    /// <param name="capacity">The capacity of this circular buffer instance.</param>
    /// <exception cref="ArgumentException">
    /// if <paramref name="capacity"/> is less than 1.
    /// </exception>
    public CircularBuffer(int capacity)
        : this(capacity, []) { }

    /// <summary>
    /// Initializes a new instance of <see cref="CircularBuffer{T}"/>
    /// </summary>
    /// <param name="capacity">The capacity of this circular buffer instance.</param>
    /// <param name="items">The startup items.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="items"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="capacity"/> is less than 1. Or
    /// if length of <paramref name="items"/> is greater than
    /// <paramref name="capacity"/>.
    /// </exception>
    public CircularBuffer(int capacity, T[] items) {
        Prevent.Argument.Null(items, nameof(items));

        if (capacity < 1) {
            throw new ArgumentException("Cannot be lower than 1.", nameof(capacity));
        }

        if (items.Length > capacity) {
            throw new ArgumentException($"Too many items. Maximum number of items must be less or equal to {nameof(capacity)} => {capacity}", nameof(items));
        }

        // let's create a copy of the current buffer.
        _buffer = new T[capacity];

        // copy all items to our inner buffer.
        Array.Copy(sourceArray: items,
                   destinationArray: _buffer,
                   length: items.Length);

        Count = items.Length;

        _start = 0;

        // if our total count is different from the
        // capacity request, set the end to the
        // Count (any new item will be added from here)
        // Otherwise; set the end and the start, this
        // will override the items as we add new ones.
        _end = Count != capacity ? Count : 0;
    }

    /// <inheritdoc />
    public void Add(T element) {
        _buffer[_end] = element;
        Increment(ref _end);

        if (IsFull) { _start = _end; } else { ++Count; }
    }

    /// <inheritdoc />
    public void Clear()
        => Count = _start = _end = 0;

    /// <inheritdoc />
    public bool Contains(T element)
        => IndexOf(element) != -1;

    /// <inheritdoc />
    public int IndexOf(T element) {
        for (var index = 0; index < Count; index++) {
            if (Equals(this[index], element)) {
                return index;
            }
        }

        return -1;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="array"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// if the difference between <paramref name="array"/> length
    /// and <paramref name="startIndex"/> were less than <see cref="Count"/>.
    /// Meaning that the <paramref name="array"/> does not have enough
    /// space for the current circular buffer items.
    /// </exception>
    public void CopyTo(T[] array, int startIndex) {
        Prevent.Argument.Null(array, nameof(array));

        if (array.Length - startIndex < Count) {
            throw new InvalidOperationException("Array does not contain enough space for items");
        }

        for (var index = 0; index < Count; ++index) {
            array[index + startIndex] = this[index];
        }
    }

    /// <inheritdoc />
    public T[] ToArray() {
        if (IsEmpty) { return []; }

        var array = new T[Count];
        for (var index = 0; index < Count; ++index) {
            array[index] = this[index];
        }

        return array;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() {
        for (var index = 0; index < Count; index++) {
            yield return _buffer[index];
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

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