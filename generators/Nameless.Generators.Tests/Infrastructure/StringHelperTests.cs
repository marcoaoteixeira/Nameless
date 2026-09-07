using Nameless.Generators.Shared.Infrastructure;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Infrastructure;

[UnitTest]
public class StringHelperTests
{
    [Fact]
    public void EscapeStringLiteral_NullInput_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, StringHelpers.EscapeStringLiteral(null));
    }

    [Fact]
    public void EscapeStringLiteral_BackslashEscaped()
    {
        Assert.Equal(@"C:\\path\\file", StringHelpers.EscapeStringLiteral(@"C:\path\file"));
    }

    [Fact]
    public void EscapeStringLiteral_QuoteEscaped()
    {
        Assert.Equal("say \\\"hello\\\"", StringHelpers.EscapeStringLiteral("say \"hello\""));
    }

    [Fact]
    public void EscapeStringLiteral_NoSpecialChars_Unchanged()
    {
        Assert.Equal("hello world", StringHelpers.EscapeStringLiteral("hello world"));
    }

    [Theory]
    [InlineData("hello",        "hello")]
    [InlineData("hello world",  "hello_world")]
    [InlineData("My.Class",     "My_Class")]
    [InlineData("some-id",      "some_id")]
    [InlineData("a1b2",         "a1b2")]
    public void Sanitize_ReplacesNonAlphanumericWithUnderscore(string input, string expected)
    {
        Assert.Equal(expected, StringHelpers.Sanitize(input));
    }
}
