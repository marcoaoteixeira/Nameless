using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Nameless.Generators.Shared.Infrastructure;

public class CodeWriter {
    private readonly StringBuilder _sb = new();
    private readonly string _indent;

    private int _level;

    public CodeWriter(int spacesPerLevel = 4) {
        _indent = new string(' ', spacesPerLevel);
    }

    public CodeWriter Indent() {
        _level++;
        
        return this;
    }

    public CodeWriter Dedent() {
        if (_level > 0) { _level--; }
        
        return this;
    }

    public CodeWriter Write(string line = "") {
        return InnerWrite(line, useLineBreaker: false);
    }

    public CodeWriter WriteLine(string line = "") {
        return InnerWrite(line, useLineBreaker: true);
    }

    public IDisposable Block(string? opening = null, string closing = "}") {
        if (opening is not null) {
            WriteLine(opening);
        }

        Indent();

        return new BlockScope(this, closing);
    }

    public string GetCode(bool prettify = false) {
        var code = _sb.ToString();

        if (!prettify) { return code; }

        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot().NormalizeWhitespace();

        return root.ToFullString();
    }

    public override string ToString() {
        return _sb.ToString();
    }

    private CodeWriter InnerWrite(string line, bool useLineBreaker) {
        if (line.Length == 0) {
            if (useLineBreaker) { _sb.AppendLine(); }
            else { _sb.Append(string.Empty);}

            return this;
        }

        for (var idx = 0; idx < _level; idx++) {
            _sb.Append(_indent);
        }

        if (useLineBreaker) { _sb.AppendLine(line); }
        else { _sb.Append(line); }

        return this;
    }

    private sealed class BlockScope(CodeWriter writer, string closing) : IDisposable {
        public void Dispose() {
            writer.Dedent();
            writer.WriteLine(closing);
        }
    }
}
