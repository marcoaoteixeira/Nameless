using Lucene.Net.Analysis;

namespace Nameless.Lucene.Fixtures;

public class FakeAnalyzer : Analyzer {
    public int Priority { get; set; }

    public FakeAnalyzer(int priority) {
        Priority = priority;
    }

    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        => new NullTokenStreamComponents();
}

public class NullTokenStreamComponents : TokenStreamComponents {
    public NullTokenStreamComponents()
        : this(new NullTokenizer()) { }

    public NullTokenStreamComponents(Tokenizer source, TokenStream result)
        : base(source, result) { }

    public NullTokenStreamComponents(Tokenizer source)
        : base(source) { }
}

public class NullTokenizer : Tokenizer {
    public NullTokenizer()
        : this(TextReader.Null) { }

    public NullTokenizer(TextReader input)
        : base(input) { }

    public NullTokenizer(AttributeFactory factory, TextReader input)
        : base(factory, input) { }

    public override bool IncrementToken() => false;
}