using System.Data;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DataParameterCollectionMocker : MockerBase<IDataParameterCollection> {
    public DataParameterCollectionMocker WithAdd(int result) {
        MockInstance.Setup(mock => mock.Add(It.IsAny<object>()))
                    .Returns(result);

        return this;
    }

    public DataParameterCollectionMocker WithAdd(object value, int result) {
        MockInstance.Setup(mock => mock.Add(value))
                    .Returns(result);

        return this;
    }

    public DataParameterCollectionMocker WithEmptyGetEnumerator() {
        MockInstance.Setup(mock => mock.GetEnumerator())
                    .Returns(Enumerable.Empty<IDataParameter>().GetEnumerator);

        return this;
    }

    public DataParameterCollectionMocker WithGetEnumerator(IEnumerator<IDataParameter> enumerator) {
        MockInstance.Setup(mock => mock.GetEnumerator())
                    .Returns(enumerator);

        return this;
    }
}