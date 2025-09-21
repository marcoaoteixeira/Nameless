using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="SortedDocValues" />.
/// </summary>
public sealed class EmptySortedDocValues : SortedDocValues {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptySortedDocValues" />.
    /// </summary>
    public static SortedDocValues Instance { get; } = new EmptySortedDocValues();

    /// <inheritdoc />
    public override int ValueCount => 0;

    static EmptySortedDocValues() { }

    private EmptySortedDocValues() { }

    /// <inheritdoc />
    public override int GetOrd(int docID) {
        return 0;
    }

    /// <inheritdoc />
    public override void LookupOrd(int ord, BytesRef result) { }
}