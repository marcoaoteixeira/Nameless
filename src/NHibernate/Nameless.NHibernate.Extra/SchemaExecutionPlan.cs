namespace Nameless.NHibernate {
    public sealed class SchemaExecutionPlan {

        #region Public Static Read-Only Properties

        public static SchemaExecutionPlan Default => new();

        #endregion

        #region Public Properties

        public ExecuteSchemaOptions ExecuteSchema { get; set; } = ExecuteSchemaOptions.OnSessionFactoryResolution;
        public SchemaOutputOptions SchemaOutput { get; set; } = SchemaOutputOptions.Console;
        public string? SchemaOutputPath { get; set; }
        public bool ExecuteAgainstDatabase { get; set; } = true;
        public bool DropBeforeExecute { get; set; }

        #endregion
    }
}
