namespace Nameless.Data.MySQL {
    public sealed class DatabaseOptions {
        #region Public Static Read-Only Properties

        public static DatabaseOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; } = "Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";

        #endregion
    }
}