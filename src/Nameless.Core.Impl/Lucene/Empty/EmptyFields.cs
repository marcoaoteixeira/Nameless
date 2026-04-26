using Lucene.Net.Index;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="Fields"/>.
/// </summary>
public sealed class EmptyFields : Fields {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyFields"/>.
    /// </summary>
    public static Fields Instance { get; } = new EmptyFields();

    /// <inheritdoc />
    public override int Count => 0;

    static EmptyFields() { }

    private EmptyFields() { }

    /// <inheritdoc />
    public override IEnumerator<string> GetEnumerator() {
        return Enumerable.Empty<string>().GetEnumerator();
    }

    /// <inheritdoc />
    public override Terms GetTerms(string field) {
        return EmptyTerms.Instance;
    }
}