namespace Nameless.NHibernate {
    public sealed class ClassMappingOptions {
        #region Public Properties

        /// <summary>
        /// Gets or sets the tables name prefix. Default is <c>null</c>.
        /// </summary>
        public string TablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the database schema name. Default is "dbo"
        /// </summary>
        public string DbSchemaName { get; set; } = "dbo";

        #endregion
    }
}