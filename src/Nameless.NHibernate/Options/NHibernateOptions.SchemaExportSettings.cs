namespace Nameless.NHibernate.Options;

public sealed record SchemaExportSettings {
    public const string DEFAULT_OUTPUT_DIRECTORY_NAME = "NHibernate";

    private string? _outputDirectoryName;

    public bool ExecuteSchemaExport { get; set; } = true;

    public bool ConsoleOutput { get; set; }

    public bool FileOutput { get; set; }

    public bool Execute { get; set; } = true;

    public bool JustDrop { get; set; }

    public string OutputDirectoryName {
        get => _outputDirectoryName.WithFallback(DEFAULT_OUTPUT_DIRECTORY_NAME);
        set => _outputDirectoryName = value;
    }
}