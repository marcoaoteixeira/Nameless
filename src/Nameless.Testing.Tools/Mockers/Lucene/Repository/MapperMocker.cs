using Lucene.Net.Documents;
using Moq;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Testing.Tools.Mockers.Lucene.Repository;

public class MapperMocker : Mocker<IMapper> {
    public MapperMocker WithMap<TEntity>(TEntity returnValue)
        where TEntity : class {
        MockInstance
            .Setup(mock => mock.Map<TEntity>(It.IsAny<Document>()))
            .Returns(returnValue);

        return this;
    }

    public MapperMocker WithMap(Document returnValue) {
        MockInstance
            .Setup(mock => mock.Map(It.IsAny<It.IsAnyType>()))
            .Returns(returnValue);

        return this;
    }

    public MapperMocker WithTryGetID<TEntity>(bool returnValue, PropertyDescriptor<TEntity>? output)
        where TEntity : class {
        MockInstance
            .Setup(mock => mock.TryGetID(out output))
            .Returns(returnValue);

        return this;
    }
}
