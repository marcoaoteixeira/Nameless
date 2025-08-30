﻿using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     The options for configuring the <see cref="IFileSystem"/>.
/// </summary>
public sealed class FileSystemOptions {
    private string? _root;
  
    /// <summary>
    ///     Gets or sets the root directory for file system operations.
    /// </summary>
    /// <remarks>
    ///     Before return the value for <see cref="Root"/>, the path
    ///     associated with it is normalized.
    /// </remarks>
    public string Root {
        get => PathHelper.Normalize(_root ?? string.Empty);
        set => _root = value;
    }

    /// <summary>
    ///     Whether to allow operations outside the root directory.
    /// </summary>
    public bool AllowOperationOutsideRoot { get; set; }
}