namespace Nameless.Data {
    public class DatabaseSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the connection string. Default value is
        /// "Data Source=:memory:; Version=3; Page Size=8192;" for
        /// SQLite database.
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=:memory:; Version=3; Page Size=8192;";

        #endregion
    }
}