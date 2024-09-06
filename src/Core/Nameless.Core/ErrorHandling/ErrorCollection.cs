using System.Collections;

namespace Nameless.ErrorHandling;

public sealed class ErrorCollection : ICollection<Error> {
    private Dictionary<string, ISet<string>> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a new empty instance of <see cref="ErrorCollection"/>.
    /// </summary>
    public static ErrorCollection Empty => [];

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
        Prevent.Argument.Null(errors, nameof(errors));

        foreach (var error in errors) {
            Push(error.Key, error.Value);
        }
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
    public void Push(string code, params string[] problems) {
        Prevent.Argument.Null(code, nameof(code));
        Prevent.Argument.Null(problems, nameof(problems));

        var error = AssertError(code);

        PushProblems(error, problems);
    }

    /// <inheritdoc />
    void ICollection<Error>.Add(Error item)
        => Cache[item.Code] = item.Problems.ToHashSet();

    /// <inheritdoc />
    void ICollection<Error>.Clear()
        => Cache.Clear();

    /// <inheritdoc />
    bool ICollection<Error>.Contains(Error item)
        => Cache.ContainsKey(item.Code);

    /// <inheritdoc />
    void ICollection<Error>.CopyTo(Error[] array, int arrayIndex)
        => this.ToArray()
               .CopyTo(array, arrayIndex);

    /// <inheritdoc />
    bool ICollection<Error>.Remove(Error item)
        => Cache.Remove(item.Code);

    /// <inheritdoc />
    public IEnumerator<Error> GetEnumerator()
        => GetEnumerable()
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerable()
            .GetEnumerator();

    private static void PushProblems(ISet<string> entry, IEnumerable<string> problems) {
        foreach (var problem in problems) {
            if (string.IsNullOrWhiteSpace(problem)) {
                continue;
            }

            entry.Add(problem);
        }
    }

    private ISet<string> AssertError(string code) {
        if (!Cache.TryGetValue(code, out var value)) {
            value = new HashSet<string>();
            Cache.Add(code, value);
        }

        return value;
    }

    private IEnumerable<Error> GetEnumerable()
        => Cache.Select(item => new Error(item.Key, [.. item.Value]));
}