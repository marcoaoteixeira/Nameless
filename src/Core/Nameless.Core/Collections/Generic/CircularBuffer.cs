using System.Collections;

namespace Nameless.Collections.Generic {
    public sealed class CircularBuffer<T> : ICircularBuffer<T> {
        #region Private Read-Only Fields

        private readonly T[] _buffer;

        #endregion

        #region Private Fields

        private int _start;
        private int _end;

        #endregion

        #region Private Properties

        private bool IsEmpty => Count == 0;
        private bool IsFull => Count == Capacity;

        #endregion

        #region Public Constructors

        public CircularBuffer(int capacity)
            : this(capacity, []) { }

        public CircularBuffer(int capacity, T[] items) {
            Guard.Against.OutOfRange(value: capacity,
                                     min: 1,
                                     max: int.MaxValue,
                                     name: nameof(capacity));
            Guard.Against.Null(items, nameof(items));

            if (items.Length > capacity) {
                throw new ArgumentException($"Too many items. Maximum number of items must be less or equal to {nameof(capacity)}", nameof(items));
            }

            _buffer = new T[capacity];

            Array.Copy(sourceArray: items,
                       destinationArray: _buffer,
                       length: items.Length);

            Count = items.Length;

            _start = 0;
            _end = Count != capacity ? Count : 0;
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region ICircularBuffer<T> Members
        
        public T this[int index] {
            get => _buffer[GetIndex(index)];
            set => _buffer[GetIndex(index)] = value;
        }

        public int Count { get; private set; }

        public int Capacity => _buffer.Length;

        public void Add(T item) {
            _buffer[_end] = item;
            Increment(ref _end);

            if (IsFull) { _start = _end; } else { ++Count; }
        }

        public void Clear()
            => Count = _start = _end = 0;

        public bool Contains(T item)
            => IndexOf(item) != -1;

        public int IndexOf(T item) {
            for (var index = 0; index < Count; index++) {
                if (Equals(this[index], item)) {
                    return index;
                }
            }

            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array.Length - arrayIndex < Count) {
                throw new ArgumentException("Array does not contain enough space for items");
            }

            for (var index = 0; index < Count; ++index) {
                array[index + arrayIndex] = this[index];
            }
        }

        public T[] ToArray() {
            if (IsEmpty) { return []; }

            var array = new T[Count];
            for (var index = 0; index < Count; ++index) {
                array[index] = this[index];
            }
            return array;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerator<T> GetEnumerator() {
            for (var index = 0; index < Count; index++) {
                yield return _buffer[index];
            }
        }

        #endregion

    }
}
