namespace Nameless.NoSQL.MongoDb {

    public sealed class MongoOptions {

        #region Public Static Read-Only Properties

        public static MongoOptions Default => new();

        #endregion

        #region Public Properties

        public string ConnectionString { get; set; } = "mongodb://mongodb0.example.com:27017";
        public string Host { get; set; } = "mongodb://mongodb0.example.com";
        public int Port { get; set; } = 27017;
        public string DatabaseName { get; set; } = "Default";
        public string Username { get; set; } = "root";
        public string Password { get; set; } = "root";

        #endregion
    }
}
