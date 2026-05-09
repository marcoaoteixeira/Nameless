using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nameless.Web.Http.Endpoints.Attributes;
using Nameless.Web.Http.Endpoints.Generator;

namespace Nameless.Web.Http.Endpoints.Infrastructure;

internal static class GeneratorTestHelper
{
    // Collect references once: all assemblies in the ASP.NET Core shared framework directory
    // plus everything already loaded into the AppDomain.
    private static readonly IReadOnlyList<MetadataReference> References = BuildReferences();

    private static List<MetadataReference> BuildReferences()
    {
        // Locate the shared framework directory from a known ASP.NET Core type.
        var aspNetCoreDir = Path.GetDirectoryName(typeof(IEndpointFilter).Assembly.Location)!;

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var refs = new List<MetadataReference>();

        void TryAdd(string path)
        {
            if (!string.IsNullOrEmpty(path) && seen.Add(path) && File.Exists(path)) {
                refs.Add(MetadataReference.CreateFromFile(path));
            }
        }

        // All DLLs in the ASP.NET Core shared framework folder.
        foreach (var dll in Directory.GetFiles(aspNetCoreDir, "*.dll")) {
            TryAdd(dll);
        }

        // Everything currently loaded in the AppDomain (covers netstandard, System.*, our own libs).
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!asm.IsDynamic) {
                TryAdd(asm.Location);
            }
        }

        // Our custom attribute assembly must always be present.
        TryAdd(typeof(EndpointAttribute<Get>).Assembly.Location);

        return refs;
    }

    internal static (ImmutableArray<Diagnostic> Diagnostics, string GeneratedSource) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new EndpointsGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out _,
            out _);

        var result = driver.GetRunResult();

        var generatedSource = result.GeneratedTrees
            .Select(static t => t.ToString())
            .FirstOrDefault() ?? string.Empty;

        return (result.Diagnostics, generatedSource);
    }

    internal static ImmutableArray<Diagnostic> GetDiagnostics(string source) {
        return RunGenerator(source).Diagnostics;
    }

    internal static string GetGeneratedSource(string source) {
        return RunGenerator(source).GeneratedSource;
    }
}
