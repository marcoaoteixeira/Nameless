using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public sealed class NullBits : IBits {
    public static IBits Instance { get; } = new NullBits();

    static NullBits() { }

    private NullBits() { }

    public int Length => 0;

    public bool Get(int index) {
        return false;
    }
}