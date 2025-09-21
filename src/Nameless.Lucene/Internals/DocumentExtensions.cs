using Lucene.Net.Util;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="IDocument" /> extension methods.
/// </summary>
internal static class DocumentExtensions {
    /// <summary>
    ///     Converts <see cref="IDocument" /> to <see cref="LuceneDocument" />.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IDocument" />.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="LuceneDocument" />.
    /// </returns>
    internal static LuceneDocument ToDocument(this IDocument self) {
        Guard.Against.Null(self);

        var result = new LuceneDocument();
        var fields = self.Select(item => item.ToIndexable());

        result.Fields.AddRange(fields);

        return result;
    }
}