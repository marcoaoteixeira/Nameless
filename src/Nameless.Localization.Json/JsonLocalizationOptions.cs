namespace Nameless.Localization.Json;

/// <summary>
/// The localization options.
/// </summary>
public sealed class JsonLocalizationOptions {
    /// <summary>
    ///     Gets or sets the name of the folder where the JSON resources files
    ///     will be located.
    /// </summary>
    public string ResourcesFolderName { get; set; } = "Localization";

    /// <summary>
    ///     Whether it will watch the JSON resource files for changes
    ///     and reload if necessary.
    /// </summary>
    public bool WatchFileForChanges { get; set; } = true;

    /// <summary>
    ///     Whether it will remove the arity sign (`) from generic
    ///     types. Default is <see langword="true"/>.
    /// </summary>
    public bool RemoveArityFromGenerics { get; set; } = true;
}