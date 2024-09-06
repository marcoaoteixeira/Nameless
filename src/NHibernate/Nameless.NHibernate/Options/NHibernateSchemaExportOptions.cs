namespace Nameless.NHibernate.Options;

public sealed record NHibernateSchemaExportOptions {
    #region Public Static Read-Only Properties

    public static NHibernateSchemaExportOptions Default => new();

    #endregion

    #region Public Properties

    public bool ExecuteSchemaExport { get; set; } = true;
    public bool ConsoleOutput { get; set; }
    public bool FileOutput { get; set; }
    public bool DropBeforeExecution { get; set; }
    public string OutputFolderName { get; set; } = "NHibernate";

    #endregion
}