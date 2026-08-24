using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Nameless.IO.Explorer;

/// <summary>
///     Current implementation of <see cref="IFileExplorerProvider"/>.
/// </summary>
public class FileExplorerProvider : IFileExplorerProvider {
    private static readonly ConcurrentDictionary<string, IFileExplorer> Cache = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public IFileExplorer Create(string root, bool allowOperationOutsideRoot = false) {
        var opts = new FileExplorerOptions {
            Root = root,
            AllowOperationOutsideRoot = allowOperationOutsideRoot
        }.Validate();

        return Cache.GetOrAdd(opts.Root, _ => new FileExplorer(
            Options.Create(opts)
        ));
    }
}