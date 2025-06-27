using System.Data;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DbCommandMocker : Mocker<IDbCommand> {
    public DbCommandMocker WithCreateParameter(IDbDataParameter parameter) {
        MockInstance.Setup(mock => mock.CreateParameter())
                    .Returns(parameter);

        return this;
    }

    public DbCommandMocker WithEmptyParameters() {
        MockInstance.Setup(mock => mock.Parameters)
                    .Returns(Mock.Of<IDataParameterCollection>());

        return this;
    }

    public DbCommandMocker WithParameters(IDataParameterCollection collection) {
        MockInstance.Setup(mock => mock.Parameters)
                    .Returns(collection);

        return this;
    }

    public DbCommandMocker WithExecuteNonQuery(int result = 0) {
        MockInstance.Setup(mock => mock.ExecuteNonQuery())
                    .Returns(result);

        return this;
    }

    public DbCommandMocker WithExecuteNonQuery<TException>() where TException : Exception, new() {
        MockInstance.Setup(mock => mock.ExecuteNonQuery())
                    .Throws<TException>();

        return this;
    }

    public DbCommandMocker WithExecuteScalar(object result = null) {
        MockInstance.Setup(mock => mock.ExecuteScalar())
                    .Returns(result);

        return this;
    }

    public DbCommandMocker WithExecuteScalar<TException>() where TException : Exception, new() {
        MockInstance.Setup(mock => mock.ExecuteScalar())
                    .Throws<TException>();

        return this;
    }

    public DbCommandMocker WithExecuteReader(IDataReader result) {
        MockInstance.Setup(mock => mock.ExecuteReader())
                    .Returns(result);

        return this;
    }

    public DbCommandMocker WithExecuteReader<TException>() where TException : Exception, new() {
        MockInstance.Setup(mock => mock.ExecuteReader())
                    .Throws<TException>();

        return this;
    }
}