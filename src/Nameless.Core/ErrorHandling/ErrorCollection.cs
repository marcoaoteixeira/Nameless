using System.Collections;

namespace Nameless.ErrorHandling;

public sealed class ErrorCollection : ICollection<Error> {
    private Dictionary<string, HashSet<string>> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string[] this[string code] {
        get => Get(code);
        set => Set(code, value);
    }

    /// <inheritdoc />
    public int Count => Cache.Count;

    /// <inheritdoc />
    bool ICollection<Error>.IsReadOnly => false;

    /// <summary>
    /// Initializes a new instance of <see cref="ErrorCollection"/>.
    /// </summary>
    public ErrorCollection() { }

    /// <summary>
    /// Initializes a new instance of <see cref="ErrorCollection"/>
    /// with the specified errors.
    /// </summary>
    /// <param name="errors">The error dictionary.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="errors"/> is <c>null</c>.
    /// </exception>
    public ErrorCollection(IDictionary<string, string[]> errors) {
        Prevent.Argument.Null(errors);

        foreach (var error in errors) {
            Add(error.Key, error.Value);
        }
    }

    /// <inheritdoc />
    public void Add(Error item) {
        Prevent.Argument.Null(item);

        Add(item.Code, item.Problems);
    }

    /// <summary>
    /// Adds a new error into the collection.
    /// </summary>
    /// <param name="code">The code of the error.</param>
    /// <param name="problems">The problems related to the error.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="code"/> or
    /// <paramref name="problems"/> is <c>null</c>.
    /// </exception>
    public void Add(string code, string[] problems) {
        Prevent.Argument.Null(code);
        Prevent.Argument.Null(problems);

        var problemSet = GetOrCreateProblemSet(code);
        foreach (var problem in problems) {
            if (string.IsNullOrWhiteSpace(problem)) {
                continue;
            }
            problemSet.Add(problem);
        }
    }

    /// <inheritdoc />
    public void Clear() => Cache.Clear();

    /// <inheritdoc />
    public bool Contains(Error item) {
        Prevent.Argument.Null(item);

        return Contains(item.Code);
    }

    public bool Contains(string code) {
        Prevent.Argument.Null(code);

        return Cache.ContainsKey(code);
    }

    /// <inheritdoc />
    public void CopyTo(Error[] array, int arrayIndex) {
        Prevent.Argument.Null(array);
        Prevent.Argument.LowerThan(arrayIndex, minValue: 0);

        var sourceArray = GetErrors().ToArray();
        
        Array.Copy(sourceArray: sourceArray,
                   sourceIndex: 0,
                   destinationArray: array,
                   destinationIndex: arrayIndex,
                   length: sourceArray.Length);
    }

    /// <inheritdoc />
    public bool Remove(Error item) {
        Prevent.Argument.Null(item);

        return Remove(item.Code);
    }

    public bool Remove(string code) {
        Prevent.Argument.Null(code);

        return Cache.Remove(code);
    }

    /// <inheritdoc />
    public IEnumerator<Error> GetEnumerator()
        => GetErrors().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetErrors().GetEnumerator();

    private IEnumerable<Error> GetErrors()
        => Cache.Select(item => new Error(item.Key, [.. item.Value]));

    private HashSet<string> GetOrCreateProblemSet(string code) {
        if (!Cache.TryGetValue(code, out var value)) {
            value = [];
            Cache.Add(code, value);
        }

        return value;
    }

    private string[] Get(string code) {
        Prevent.Argument.Null(code);

        if (!Contains(code)) {
            throw new KeyNotFoundException($"Could not found error with code {code}");
        }

        return Cache[code].ToArray();
    }

    private void Set(string code, string[] problems)
        => Add(code, problems);
}