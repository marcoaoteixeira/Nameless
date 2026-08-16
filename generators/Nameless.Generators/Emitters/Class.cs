namespace Nameless.Generators.Emitters;

public readonly record struct Class {
    public string HintName { get; }
    public string SourceCode { get; }

    public Class(string hintName, string sourceCode) {
        HintName = hintName;
        SourceCode = sourceCode;
    }
}