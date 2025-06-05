using System.Data;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DataReaderMocker : MockerBase<IDataReader> {
    public DataReaderMocker WithReadSequence(params bool[] sequence) {
        var setup = MockInstance.SetupSequence(mock => mock.Read());

        foreach (var item in sequence) {
            setup.Returns(item);
        }

        return this;
    }
}