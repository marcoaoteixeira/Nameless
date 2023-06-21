namespace Nameless.Data.SQLite {
    public sealed class DatabaseOptions {
        #region Public Static Read-Only Properties

        public static DatabaseOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=:memory:; Version=3; Page Size=8192;";

        #endregion
    }
}