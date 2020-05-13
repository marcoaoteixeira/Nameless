using Xunit;

namespace Nameless.Search.Lucene.Test {
    public class IndexProviderTest {
        [Fact]
        public void Can_Create () {
            // arrange
            IIndexProvider indexProvider;
            var defaultAnalyzerSelector = new DefaultAnalyzerSelector ();
            var analyzerProvider = new AnalyzerProvider (new [] { defaultAnalyzerSelector });
            var settings = new SearchSettings ();

            // act
            indexProvider = new IndexProvider (analyzerProvider, settings);
            var index = indexProvider.GetOrCreate ("Temp");

            // assert
            Assert.NotNull (index);
        }

        [Fact]
        public void Can_Delete () {
            // arrange
            IIndexProvider indexProvider;
            var defaultAnalyzerSelector = new DefaultAnalyzerSelector ();
            var analyzerProvider = new AnalyzerProvider (new [] { defaultAnalyzerSelector });
            var settings = new SearchSettings ();

            // act
            indexProvider = new IndexProvider (analyzerProvider, settings);
            indexProvider.GetOrCreate ("Temp");
            var indexExistsBefore = indexProvider.Exists ("Temp");

            indexProvider.Delete ("Temp");
            var indexExistsAfter = indexProvider.Exists ("Temp");

            // assert
            Assert.True (indexExistsBefore, "Index should exists first");
            Assert.False (indexExistsAfter, "Index shouldn't exists after deletion");
        }
    }
}