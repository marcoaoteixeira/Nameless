namespace Nameless.Lucene;

public static class IndexProviderExtensions {
    extension(IIndexProvider self) {
        public IIndex Get() {
            return self.Get(indexName: null);
        }
    }
}
