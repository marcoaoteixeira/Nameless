namespace Nameless.Localization.Json.Options;

public sealed class LocalizationOptions {
    public const string DEFAULT_TRANSLATION_FOLDER_NAME = "Localization";

    private string? _translationFolderName;
    /// <summary>
    /// Gets or sets the name of the folder where the translation files
    /// should be look for. Relative to the application base path.
    /// This property is <strong>required</strong>.
    /// </summary>
    /// <remarks>
    /// Value will be <see cref="DEFAULT_TRANSLATION_FOLDER_NAME"/> if it
    /// is not provided.
    /// </remarks>
    public string TranslationFolderName {
        get => _translationFolderName.WithFallback(DEFAULT_TRANSLATION_FOLDER_NAME);
        set => _translationFolderName = value;
    }

    /// <summary>
    /// Gets or sets whether it will watch the translation files for changes
    /// and reload if necessary.
    /// </summary>
    public bool WatchFileForChanges { get; set; } = true;
}