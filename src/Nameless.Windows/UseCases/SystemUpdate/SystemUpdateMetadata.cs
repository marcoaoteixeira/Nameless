namespace Nameless.Windows.UseCases.SystemUpdate;

/// <summary>
///     System update response metadata
/// </summary>
/// <param name="IsAvailable">
///     Whether is there an update or not.
/// </param>
/// <param name="Version">
///     When update is available, provides the latest version information.
/// </param>
/// <param name="ZipFilePath">
///     The path to the zip file that contains the system update.
/// </param>
public readonly record struct SystemUpdateMetadata(
    bool IsAvailable,
    string Version,
    string ZipFilePath
);