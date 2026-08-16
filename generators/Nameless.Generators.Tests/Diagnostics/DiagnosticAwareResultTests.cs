using Microsoft.CodeAnalysis;
using Nameless.Generators.Infrastructure;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Diagnostics;

[UnitTest]
public class DiagnosticAwareResultTests
{
    [Fact]
    public void ImplicitFromModel_IsSuccessful_ModelSet()
    {
        DiagnosticAwareResult<string> result = "value";

        Assert.True(result.Successful);
        Assert.Equal("value", result.Model);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void ImplicitFromDiagnostic_NotSuccessful_ModelNull()
    {
        DiagnosticAwareResult<string> result = CreateDiagnostic();

        Assert.False(result.Successful);
        Assert.Null(result.Model);
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void ImplicitFromDiagnosticArray_NotSuccessful()
    {
        DiagnosticAwareResult<string> result = new[] { CreateDiagnostic(), CreateDiagnostic() };

        Assert.False(result.Successful);
        Assert.Equal(2, result.Diagnostics.Length);
    }

    [Fact]
    public void ImplicitFromTuple_WithNoDiagnostics_IsSuccessful()
    {
        DiagnosticAwareResult<string> result = ("value", []);

        Assert.True(result.Successful);
        Assert.Equal("value", result.Model);
    }

    [Fact]
    public void ImplicitFromTuple_WithDiagnostics_NotSuccessful()
    {
        DiagnosticAwareResult<string> result = ("value", [CreateDiagnostic()]);

        // Successful is driven solely by Diagnostics.Length == 0
        Assert.False(result.Successful);
        Assert.Equal("value", result.Model);
        Assert.Single(result.Diagnostics);
    }

    private static GeneratorDiagnostic CreateDiagnostic() => GeneratorDiagnostic.Create(
        descriptor: new DiagnosticDescriptor(
            id: "TEST001",
            title: "Test",
            messageFormat: "Test message",
            category: "Test",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        ),
        location: default,
        messageArgs: []
    );
}
