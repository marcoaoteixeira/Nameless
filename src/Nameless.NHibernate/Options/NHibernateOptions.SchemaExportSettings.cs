namespace Nameless.NHibernate.Options;

public sealed record SchemaExportSettings {
    public const string DefaultOutputFolderName = "NHibernate";

    private string? _outputFolderName;

    public bool ExecuteSchemaExport { get; set; } = true;

    public bool ConsoleOutput { get; set; }

    public bool FileOutput { get; set; }

    public bool Execute { get; set; } = true;

    public bool JustDrop { get; set; }

    public string OutputFolderName {
        get => _outputFolderName.WithFallback(DefaultOutputFolderName);
        set => _outputFolderName = value;
    }
}