using System.Data;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DbConnectionMocker : MockerBase<IDbConnection> {
    public DbConnectionMocker WithCreateCommand(IDbCommand command) {
        MockInstance.Setup(mock => mock.CreateCommand())
                    .Returns(command);

        return this;
    }

    public DbConnectionMocker WithBeginTransaction(IDbTransaction result) {
        MockInstance.Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                    .Returns(result);

        return this;
    }
}