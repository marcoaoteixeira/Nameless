# Nameless.Collections.Generic Documentation

## Content
- [CircularBuffer Class](#circularbuffer-class)
- [Page Class](#page-class)

<a id="circularbuffer-class"></a>
## CircularBuffer Class

The `CircularBuffer<TElement>` class is a generic circular buffer implementation that allows for efficient storage and retrieval of elements in a fixed-size buffer. It is particularly useful for scenarios where you need to maintain a limited amount of data in memory, such as in streaming applications or real-time data processing.

### Methods and Properties

- `this[int index]`: Gets or sets the item at the specified index in the circular buffer. The index is treated as a circular index, meaning it wraps around if it exceeds the buffer size.
- `Count`: Gets the number of items currently in the circular buffer.
- `Capacity`: Gets the maximum number of items that can be stored in the circular buffer.
- `Add(TElement element)`: Adds an item to the circular buffer. If the buffer is full, the oldest item will be overwritten.
- `Clear()`: Clears the circular buffer, removing all items.
- `Contains(TElement element)`: Checks if the circular buffer contains a specific item.
- `IndexOf(TElement element)`: Returns the index of the first occurrence of a specific item in the circular buffer, or -1 if not found.
- `CopyTo(TElement[] array, int startIndex)`: Copies the elements of the circular buffer to a one-dimensional array, starting at the specified index.
- `ToArray()`: Converts the circular buffer to an array.

<a id="page-class"></a>
## Page Class

The `Page<TItem>` class is a generic implementation of a page that can hold a collection of elements. It is designed to be used in scenarios where you need to manage a large number of items and want to break them into smaller, manageable pages.

### Methods and Properties

- `Number`: Gets the page number.
- `Size`: Gets the size of the page, which determines how many items it can hold.
- `TotalItems`: Gets the total number of items across all possible pages for the provided query.
- `TotalPages`: Gets the total number of pages based on the total number of items and the page size.
- `Items`: Returns an array of `TItem` in the page.
- `Count`: Gets the number of items currently in the page.
- `HasPrevious`: Indicates whether there is a previous page.
- `HasNext`: Indicates whether there is a next page.
- `GetEnumerator<TItem>()`: Returns an enumerator that iterates through the page.`

### Remarks

The `Page<TItem>` behaves like a collection ( :duck: ), allowing you to use it in a `foreach` loop.

## QueryableExtensions

The `QueryableExtensions` class provides extension methods for `IQueryable<T>` regarding the namespace `Nameless.Collections.Generic`.

### Available Extension Methods

- `CreatePage<TItem>(this IQueryable<TItem> self, int number, int size)`: Creates a `IPage<TItem>` from a `IQueryable<TItem>`, allowing for pagination of the results.