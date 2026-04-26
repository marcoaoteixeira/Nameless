using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="BinaryDocValues" />.
/// </summary>
public sealed class EmptyBinaryDocValues : BinaryDocValues {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyBinaryDocValues" />.
    /// </summary>
    public static BinaryDocValues Instance { get; } = new EmptyBinaryDocValues();

    static EmptyBinaryDocValues() { }

    private EmptyBinaryDocValues() { }

    /// <inheritdoc />
    public override void Get(int docID, BytesRef result) { }
}