using Nameless.Data;
using Nameless.Data.SQLite;

namespace Nameless.EventSourcing.Test {
    public abstract class DatabaseAwareTest {
        protected IDatabase CreateDatabase () {
            DatabaseSettings settings = new DatabaseSettings {
                ConnectionString = "Data Source=:memory:;Version=3;New=True;"
            };
            IDbConnectionFactory dbConnectionFactory = new DbConnectionFactory (settings);
            IDatabase database = new Database (dbConnectionFactory);
            return database;
        }
    }
}
