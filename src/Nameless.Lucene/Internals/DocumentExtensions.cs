using Lucene.Net.Index;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="Document" /> extension methods.
/// </summary>
internal static class DocumentExtensions {
    /// <summary>
    ///     Converts <see cref="Document" /> to <see cref="IEnumerable{T}" />
    ///     of <see cref="IIndexableField"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Document" />.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="IIndexableField" />.
    /// </returns>
    internal static IEnumerable<IIndexableField> ToIndexableFields(this Document self) {
        return self.Select(item => item.ToIndexableField());
    }
}