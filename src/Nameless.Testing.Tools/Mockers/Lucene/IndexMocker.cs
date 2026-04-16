using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Moq;
using Nameless.Lucene;
using Nameless.Lucene.ObjectModel;
using Nameless.Results;

namespace Nameless.Testing.Tools.Mockers.Lucene;

public class IndexMocker : Mocker<IIndex> {
    public IndexMocker WithInsert(Result<bool>? returnValue = null, Delegate? callback = null) {
        var setup = MockInstance
                    .Setup(mock => mock.Insert(It.IsAny<DocumentCollection>()));

        if (callback is not null) {
            setup.Callback(callback);
        }

        setup.Returns(returnValue ?? true);

        return this;
    }

    public IndexMocker WithDelete(Result<bool>? returnValue = null) {
        MockInstance
            .Setup(mock => mock.Delete(It.IsAny<Query>()))
            .Returns(returnValue ?? true);

        return this;
    }

    public IndexMocker WithUpdate(Result<bool>? returnValue = null, Delegate? callback = null) {
        var setup = MockInstance
            .Setup(mock => mock.Update(It.IsAny<Term>(), It.IsAny<Document>()));

        if (callback is not null) {
            setup.Callback(callback);
        }

        setup.Returns(returnValue ?? true);

        return this;
    }

    public IndexMocker WithSearch(params IEnumerable<ScoreDocument> returnValue) {
        MockInstance
            .Setup(mock => mock.Search(It.IsAny<Query>(), It.IsAny<Sort>(), It.IsAny<int>()))
            .Returns(returnValue);

        return this;
    }

    public IndexMocker WithRollback(Result<bool>? returnValue = null) {
        MockInstance
            .Setup(mock => mock.Rollback())
            .Returns(returnValue ?? true);

        return this;
    }

    public IndexMocker WithSaveChanges(Result<bool>? returnValue = null) {
        MockInstance
            .Setup(mock => mock.SaveChanges())
            .Returns(returnValue ?? true);

        return this;
    }
}
