using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="IIndexableField"/>.
/// </summary>
public sealed class EmptyIndexableField : IIndexableField {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyIndexableField"/>.
    /// </summary>
    public static IIndexableField Instance { get; } = new EmptyIndexableField();

    /// <inheritdoc />
    public string Name => string.Empty;

    /// <inheritdoc />
    public IIndexableFieldType IndexableFieldType => new FieldType();

    /// <inheritdoc />
    public float Boost => 0F;

    /// <inheritdoc />
    public NumericFieldType NumericType => NumericFieldType.NONE;

    static EmptyIndexableField() { }

    private EmptyIndexableField() { }

    /// <inheritdoc />
    public BytesRef GetBinaryValue() {
        return new BytesRef(BytesRef.EMPTY_BYTES);
    }

    /// <inheritdoc />
    public string GetStringValue() {
        return string.Empty;
    }

    /// <inheritdoc />
    public string GetStringValue(IFormatProvider provider) {
        return string.Empty;
    }

    /// <inheritdoc />
    public string GetStringValue(string format) {
        return string.Empty;
    }

    /// <inheritdoc />
    public string GetStringValue(string format, IFormatProvider provider) {
        return string.Empty;
    }

    /// <inheritdoc />
    public TextReader GetReaderValue() {
        return TextReader.Null;
    }

    /// <inheritdoc />
    public object GetNumericValue() {
        return 0;
    }

    /// <inheritdoc />
    public byte? GetByteValue() {
        return null;
    }

    /// <inheritdoc />
    public short? GetInt16Value() {
        return null;
    }

    /// <inheritdoc />
    public int? GetInt32Value() {
        return null;
    }

    /// <inheritdoc />
    public long? GetInt64Value() {
        return null;
    }

    /// <inheritdoc />
    public float? GetSingleValue() {
        return null;
    }

    /// <inheritdoc />
    public double? GetDoubleValue() {
        return null;
    }

    /// <inheritdoc />
    public TokenStream GetTokenStream(Analyzer analyzer) {
        return new EmptyTokenStream();
    }
}