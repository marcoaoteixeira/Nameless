using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Nameless.Web.Http.Endpoints;

namespace Nameless.Generators.Web.Http.Endpoints;

public static class CodeGeneratorHelper {
    private const string ASSEMBLY_NAME = "TestAssembly";

    // Collect references once: all assemblies in the ASP.NET Core shared
    // framework directory plus everything already loaded into the AppDomain.
    private static readonly IReadOnlyList<MetadataReference> References = CollectReferences();

    private static List<MetadataReference> CollectReferences()
    {
        var aspNetCoreDir = Path.GetDirectoryName(typeof(IEndpointFilter).Assembly.Location);
        if (string.IsNullOrWhiteSpace(aspNetCoreDir))
        {
            throw new InvalidOperationException("Can't find ASPNET Core assemblies location");
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var refs = new List<MetadataReference>();

        foreach (var dll in Directory.GetFiles(aspNetCoreDir, "*.dll"))
        {
            TryAdd(dll);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic)
            {
                TryAdd(assembly.Location);
            }
        }

        TryAdd(typeof(EndpointAttribute).Assembly.Location);

        return refs;

        void TryAdd(string path)
        {
            if (!string.IsNullOrEmpty(path) && seen.Add(path) && File.Exists(path))
            {
                refs.Add(MetadataReference.CreateFromFile(path));
            }
        }
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, Dictionary<SourceType, string[]> GeneratedSource) RunGenerator(string source, string? assemblyName = null, bool useAutoEndpoints = true)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName ?? ASSEMBLY_NAME,
            syntaxTrees: [syntaxTree],
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var generator = new AutoEndpointsGenerator();

        // Mirrors what Directory.Build.targets / Nameless.Common.targets
        // do for a real consumer that opted in: expose UseAutoEndpoints as 
        // a compiler-visible build property so the generator's own
        // defense-in-depth check (see AutoEndpointsGenerator.Initialize) sees it.
        // 
        var optionsProvider = new TestAnalyzerConfigOptionsProvider(
            new TestAnalyzerConfigOptions(
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    { "build_property.UseAutoEndpoints", useAutoEndpoints.ToString().ToLowerInvariant() }
                }
            )
        );

        var driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            optionsProvider: optionsProvider
        );

        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation: compilation,
            outputCompilation: out _,
            diagnostics: out _
        );

        var result = driver.GetRunResult();

        var sources = result.GeneratedTrees
            .GroupBy(
                keySelector: item => ExtractSourceType(item.ToString()),
                elementSelector: item => item.ToString()
            )
            .ToDictionary(
                keySelector: item => item.Key,
                elementSelector: item => item.ToArray()
            );

        return (result.Diagnostics, sources);

        static SourceType ExtractSourceType(string code)
        {
            if (code.Contains("RegisterAutoEndpoints"))
            {
                return SourceType.Registration;
            }

            if (code.Contains("MapGroup"))
            {
                return SourceType.EndpointGroup;
            }

            return SourceType.Endpoint;
        }
    }

    public static ImmutableArray<Diagnostic> GetDiagnostics(string source, string? assemblyName = null)
    {
        return RunGenerator(source, assemblyName: assemblyName).Diagnostics;
    }

    /// <summary>
    ///     Concatenate all generated files so tests can
    ///     Assert.Contains across the full output.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="assemblyName">Assembly name.</param>
    /// <returns>
    ///     The generated output
    /// </returns>
    public static string GetCode(string source, string? assemblyName = null)
    {
        var (_, sources) = RunGenerator(source, assemblyName: assemblyName);

        var composite = string.Join(
            separator: Environment.NewLine,
            values: sources.Values.SelectMany(item => item)
        );

        return composite;
    }

    public static string[] GetCodeBySourceType(string source, SourceType type = SourceType.Endpoint, string? assemblyName = null)
    {
        var (_, sources) = RunGenerator(source, assemblyName: assemblyName);

        return sources.TryGetValue(type, out var output) ? output : [];
    }

    private sealed class TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions opts) : AnalyzerConfigOptionsProvider {
        public override AnalyzerConfigOptions GlobalOptions { get; } = opts;
        
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) {
            return GlobalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) {
            return GlobalOptions;
        }
    }

    private sealed class TestAnalyzerConfigOptions(Dictionary<string, string> values) : AnalyzerConfigOptions {
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) {
            return values.TryGetValue(key, out value);
        }
    }
}

public enum SourceType
{
    Endpoint,

    EndpointGroup,

    Registration
}
