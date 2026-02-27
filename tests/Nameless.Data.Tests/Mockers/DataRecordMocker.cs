using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moq;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Mockers;

public class DataRecordMocker : Mocker<IDataRecord> {
    public DataRecordMocker(bool useSequence = false)
        : base(useSequence ? new MockSequence() : null) {
    }

    public DataRecordMocker WithIndexer(string key, object value) {
        MockInstance
            .Setup(mock => mock[key])
            .Returns(value);

        return this;
    }
}