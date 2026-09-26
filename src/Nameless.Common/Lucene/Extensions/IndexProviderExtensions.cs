namespace Nameless.Lucene;

/// <summary>
///     <see cref="IIndexProvider"/> extension methods.
/// </summary>
public static class IndexProviderExtensions {
    /// <param name="self">The current <see cref="IIndexProvider"/> instance.</param>
    extension(IIndexProvider self) {
        /// <summary>
        ///     Retrieves the default <see cref="IIndex"/> instance.
        /// </summary>
        /// <returns>The default <see cref="IIndex"/> instance.</returns>
        public IIndex Get() {
            return self.Get(indexName: null);
        }
    }
}
