using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="ISession"/> that does not store any session data.
/// </summary>
public sealed class NullSession : ISession {
    public static ISession Instance { get; } = new NullSession();

    /// <inheritdoc />
    public bool IsAvailable => false;

    /// <inheritdoc />
    public string Id => string.Empty;

    /// <inheritdoc />
    public IEnumerable<string> Keys => [];

    static NullSession() { }

    private NullSession() { }

    /// <inheritdoc />
    public Task LoadAsync(CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task CommitAsync(CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, [NotNullWhen(returnValue: true)] out byte[]? value) {
        value = null;

        return false;
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value) { }

    /// <inheritdoc />
    public void Remove(string key) { }

    /// <inheritdoc />
    public void Clear() { }
}