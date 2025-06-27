using System.Data;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DbConnectionFactoryMocker : Mocker<IDbConnectionFactory> {
    public DbConnectionFactoryMocker WithCreateDbConnection(IDbConnection result) {
        MockInstance.Setup(mock => mock.CreateDbConnection())
                    .Returns(result);

        return this;
    }
}