using System.Data;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;
public class DataRecordMocker : MockerBase<IDataRecord> {
    public DataRecordMocker() : base(MockBehavior.Strict) { }

    public DataRecordMocker WithIndexer(string key, object value) {
        MockInstance
           .Setup(mock => mock[key])
           .Returns(value);

        return this;
    }
}
