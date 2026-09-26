using Nameless.Generators.Shared.Emitters;
using Nameless.Generators.Shared.Infrastructure;

namespace Nameless.Generators.EventSourcing.Emitters;

public abstract class Emitter<TModel> : EmitterBase<TModel>
    where TModel : IEmitModel {
    protected override void WriteNamespaceUsings(CodeWriter cw, TModel model) {
        cw.WriteLine("using global::Nameless.EventSourcing;");

        base.WriteNamespaceUsings(cw, model);
    }
}
