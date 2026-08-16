using Nameless.Generators.Emitters;
using Nameless.Generators.Infrastructure;

namespace Nameless.Generators.EventSourcing.Emitters;

public abstract class Emitter<TModel> : EmitterBase<TModel>
    where TModel : IEmitModel {
    protected override void EmitUsingBlock(CodeWriter cw, TModel model) {
        cw.WriteLine("using global::Nameless.EventSourcing;");

        base.EmitUsingBlock(cw, model);
    }
}
