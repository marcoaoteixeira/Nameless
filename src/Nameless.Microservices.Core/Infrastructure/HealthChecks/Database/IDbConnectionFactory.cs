using System.Data.Common;

namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

public interface IDbConnectionFactory {
    DbConnection CreateConnection();
}