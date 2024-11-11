namespace Nameless.Localization.Json.Options;

public sealed class LocalizationOptions {
    public const string DEFAULT_RESOURCES_FOLDER_NAME = "Localization";

    private string? _resourcesFolderName;
    /// <summary>
    /// Gets or sets the name of the folder where the JSON resources files
    /// will be located. (This is relative to the application base path)
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="DEFAULT_RESOURCES_FOLDER_NAME"/>
    /// </remarks>
    public string ResourcesFolderName {
        get => _resourcesFolderName.WithFallback(DEFAULT_RESOURCES_FOLDER_NAME);
        set => _resourcesFolderName = value;
    }

    /// <summary>
    /// Whether it will watch the JSON resource files for changes
    /// and reload if necessary.
    /// </summary>
    public bool WatchFileForChanges { get; set; } = true;

    /// <summary>
    /// Whether it will remove the arity sign (`) from generic
    /// types. Default is <c>true</c>.
    /// </summary>
    public bool RemoveArityFromGenerics { get; set; } = true;
}