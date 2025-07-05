using System.Collections;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IFormFileCollection"/> that does not store any files.
/// </summary>
public sealed class NullFormFileCollection : IFormFileCollection {
    public static IFormFileCollection Instance { get; } = new NullFormFileCollection();

    /// <inheritdoc />
    public IFormFile this[string name] => NullFormFile.Instance;

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public IFormFile this[int index] => NullFormFile.Instance;

    static NullFormFileCollection() { }

    private NullFormFileCollection() { }

    /// <inheritdoc />
    public IEnumerator<IFormFile> GetEnumerator() {
        return Enumerable.Empty<IFormFile>().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IFormFile GetFile(string name) {
        return NullFormFile.Instance;
    }

    /// <inheritdoc />
    public IReadOnlyList<IFormFile> GetFiles(string name) {
        return [];
    }
}