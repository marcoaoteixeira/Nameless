using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="SortedSetDocValues" />.
/// </summary>
public sealed class EmptySortedSetDocValues : SortedSetDocValues {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptySortedSetDocValues" />.
    /// </summary>
    public static SortedSetDocValues Instance { get; } = new EmptySortedSetDocValues();

    /// <inheritdoc />
    public override long ValueCount => 0L;

    static EmptySortedSetDocValues() { }

    private EmptySortedSetDocValues() { }

    /// <inheritdoc />
    public override long NextOrd() {
        return 0L;
    }

    /// <inheritdoc />
    public override void SetDocument(int docID) { }

    /// <inheritdoc />
    public override void LookupOrd(long ord, BytesRef result) { }
}