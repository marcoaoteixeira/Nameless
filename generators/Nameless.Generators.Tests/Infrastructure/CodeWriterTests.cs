using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Infrastructure;

[UnitTest]
public class CodeWriterTests
{
    [Fact]
    public void WriteLine_NoIndent_WritesLineWithoutLeadingSpaces()
    {
        var writer = new CodeWriter();
        writer.WriteLine("hello");

        Assert.StartsWith("hello", writer.ToString());
    }

    [Fact]
    public void WriteLine_AfterIndent_AddsLeadingSpaces()
    {
        var writer = new CodeWriter(spacesPerLevel: 4);
        writer.Indent().WriteLine("hello");

        Assert.StartsWith("    hello", writer.ToString());
    }

    [Fact]
    public void Dedent_BelowZero_ClampsAtZero()
    {
        var writer = new CodeWriter();
        writer.Dedent().Dedent().WriteLine("x");

        Assert.StartsWith("x", writer.ToString());
    }

    [Fact]
    public void Write_EmptyLine_DoesNotAddIndent()
    {
        var writer = new CodeWriter();
        writer.Indent().Write();

        // An empty write at any indent level must not prepend spaces
        Assert.Equal(string.Empty, writer.ToString());
    }

    [Fact]
    public void Block_AutoDedentsAndClosesOnDispose()
    {
        var writer = new CodeWriter(spacesPerLevel: 4);
        writer.WriteLine("outer");

        using (writer.Block("inner {"))
        {
            writer.WriteLine("body");
        }

        var code = writer.ToString();
        Assert.Contains("inner {", code);
        Assert.Contains("    body", code);
        // closing brace must be at the outer level (no indent)
        Assert.Matches(@"(?m)^\}", code);
    }

    [Fact]
    public void Block_CustomClosing_UsesProvidedClosingToken()
    {
        var writer = new CodeWriter();

        using (writer.Block(closing: "end"))
        {
            writer.WriteLine("content");
        }

        Assert.Contains("end", writer.ToString());
    }

    [Fact]
    public void GetCode_ReturnsAccumulatedCode()
    {
        var writer = new CodeWriter();
        writer.WriteLine("line1");
        writer.WriteLine("line2");

        var code = writer.GetCode();
        Assert.Contains("line1", code);
        Assert.Contains("line2", code);
    }
}
