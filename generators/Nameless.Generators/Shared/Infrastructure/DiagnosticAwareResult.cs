using Nameless.Generators.Shared.Diagnostics;

namespace Nameless.Generators.Shared.Infrastructure;

public sealed record DiagnosticAwareResult<TModel> {
    public TModel? Model { get; }
    public bool Successful => Diagnostics.Length == 0;
    public GeneratorDiagnostic[] Diagnostics { get; } = [];

    public DiagnosticAwareResult(TModel? model, GeneratorDiagnostic[] diagnostics) {
        Model = model;
        Diagnostics = diagnostics;
    }

    public static implicit operator DiagnosticAwareResult<TModel>(TModel model) {
        return new DiagnosticAwareResult<TModel> (model, diagnostics: []);
    }

    public static implicit operator DiagnosticAwareResult<TModel>(GeneratorDiagnostic diagnostic) {
        return new DiagnosticAwareResult<TModel>(model: default, diagnostics: [diagnostic]);
    }

    public static implicit operator DiagnosticAwareResult<TModel>(GeneratorDiagnostic[] diagnostics) {
        return new DiagnosticAwareResult<TModel>(model: default, diagnostics);
    }

    public static implicit operator DiagnosticAwareResult<TModel>((TModel Model, GeneratorDiagnostic[] Diagnostics) result) {
        return new DiagnosticAwareResult<TModel>(result.Model, result.Diagnostics);
    }
}