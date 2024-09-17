namespace Nameless.NHibernate.Options;

public sealed record NHibernateSchemaExportOptions {
    public const string DEFAULT_OUTPUT_FOLDER_NAME = "NHibernate";

    private string? _outputFolderName;

    public static NHibernateSchemaExportOptions Default => new();

    public bool ExecuteSchemaExport { get; set; } = true;
    
    public bool ConsoleOutput { get; set; }
    
    public bool FileOutput { get; set; }

    public bool DropBeforeExecution { get; set; }

    public string OutputFolderName {
        get => _outputFolderName.WithFallback(DEFAULT_OUTPUT_FOLDER_NAME);
        set => _outputFolderName = value;
    }
}