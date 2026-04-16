using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IFormFile"/> that does not perform any operations.
/// </summary>
public sealed class NullFormFile : IFormFile {
    public static IFormFile Instance { get; } = new NullFormFile();

    /// <inheritdoc />
    public string ContentType => string.Empty;

    /// <inheritdoc />
    public string ContentDisposition => string.Empty;

    /// <inheritdoc />
    public IHeaderDictionary Headers => NullHeaderDictionary.Instance;

    /// <inheritdoc />
    public long Length => 0;

    /// <inheritdoc />
    public string Name => string.Empty;

    /// <inheritdoc />
    public string FileName => string.Empty;

    static NullFormFile() { }

    private NullFormFile() { }

    /// <inheritdoc />
    public Stream OpenReadStream() {
        return Stream.Null;
    }

    /// <inheritdoc />
    public void CopyTo(Stream target) { }

    /// <inheritdoc />
    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = new()) {
        return Task.CompletedTask;
    }
}