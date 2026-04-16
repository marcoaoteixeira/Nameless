using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="IBits" />.
/// </summary>
public sealed class EmptyBits : IBits {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyBits" />.
    /// </summary>
    public static IBits Instance { get; } = new EmptyBits();

    static EmptyBits() { }

    private EmptyBits() { }

    /// <inheritdoc />
    public int Length => 0;

    /// <inheritdoc />
    public bool Get(int index) {
        return false;
    }
}