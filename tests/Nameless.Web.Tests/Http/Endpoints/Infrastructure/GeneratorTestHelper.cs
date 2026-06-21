using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nameless.Web.Generators;

namespace Nameless.Web.Http.Endpoints.Infrastructure;

internal enum SourceType {
    Unknown,

    Registration,

    Endpoint,

    EndpointGroup
}

internal static class GeneratorTestHelper {
    // Collect references once: all assemblies in the ASP.NET Core shared
    // framework directory plus everything already loaded into the AppDomain.
    private static readonly IReadOnlyList<MetadataReference> References = CollectReferences();

    private static List<MetadataReference> CollectReferences() {
        var aspNetCoreDir = Path.GetDirectoryName(typeof(IEndpointFilter).Assembly.Location)!;

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var refs = new List<MetadataReference>();

        foreach (var dll in Directory.GetFiles(aspNetCoreDir, "*.dll")) {
            TryAdd(dll);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (!assembly.IsDynamic) {
                TryAdd(assembly.Location);
            }
        }

        TryAdd(typeof(EndpointAttribute).Assembly.Location);

        return refs;

        void TryAdd(string path) {
            if (!string.IsNullOrEmpty(path) && seen.Add(path) && File.Exists(path)) {
                refs.Add(MetadataReference.CreateFromFile(path));
            }
        }
    }

    internal static (ImmutableArray<Diagnostic> Diagnostics, Dictionary<SourceType, string[]> Sources) RunGenerator(string source, string assemblyName = "TestAssembly") {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: [syntaxTree],
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var generator = new AutoEndpointsGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation: compilation,
            outputCompilation: out _,
            diagnostics: out _
        );

        var result = driver.GetRunResult();

        var sources = result.GeneratedTrees.GroupBy(
            keySelector: item => GetSourceType(item.ToString()),
            elementSelector: item => item.ToString()
        ).ToDictionary(
            keySelector: item => item.Key,
            elementSelector: item => item.ToArray()
        );

        return (result.Diagnostics, sources);

        static SourceType GetSourceType(string code) {
            if (code.Contains("MapGroup")) {
                return SourceType.EndpointGroup;
            }

            if (code.Contains("RegisterAutoEndpoints")) {
                return SourceType.Registration;
            }

            return SourceType.Endpoint;
        }
    }

    internal static ImmutableArray<Diagnostic> GetDiagnostics(string source) {
        return RunGenerator(source).Diagnostics;
    }

    internal static string GetGeneratedSource(string source) {
        var (_, sources) = RunGenerator(source);

        // Concatenate all generated files so tests can Assert.Contains across
        // the full output.
        var composite = string.Join(
            separator: Environment.NewLine,
            values: sources.Values.SelectMany(item => item)
        );

        return composite;
    }
}
