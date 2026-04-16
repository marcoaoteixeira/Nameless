using Lucene.Net.Index;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="NumericDocValues" />
///     that always returns 0.
/// </summary>
public sealed class EmptyNumericDocValues : NumericDocValues {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyNumericDocValues" />.
    /// </summary>
    public static NumericDocValues Instance { get; } = new EmptyNumericDocValues();

    static EmptyNumericDocValues() { }

    private EmptyNumericDocValues() { }

    /// <inheritdoc />
    public override long Get(int docID) {
        return 0L;
    }
}