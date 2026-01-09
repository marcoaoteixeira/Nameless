namespace Nameless.NHibernate.Options;

public record SchemaExportSettings {
    public const string DEFAULT_OUTPUT_DIRECTORY_NAME = "NHibernate";

    public bool ExecuteSchemaExport { get; set; } = true;

    public bool ConsoleOutput { get; set; }

    public bool FileOutput { get; set; }

    public bool Execute { get; set; } = true;

    public bool JustDrop { get; set; }

    public string OutputDirectoryName {
        get => field.WithFallback(DEFAULT_OUTPUT_DIRECTORY_NAME);
        set;
    }
}