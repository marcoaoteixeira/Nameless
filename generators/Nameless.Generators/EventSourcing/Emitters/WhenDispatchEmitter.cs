using Nameless.Generators.EventSourcing.Model;
using Nameless.Generators.Shared.Emitters;
using Nameless.Generators.Shared.Infrastructure;

namespace Nameless.Generators.EventSourcing.Emit;

/// <summary>
///     Emits the <c>When(IEvent)</c> override for an aggregate class from
///     its <c>[Apply]</c> methods.
/// </summary>
public class WhenDispatchEmitter : EmitterBase<AggregateModel> {
    public static WhenDispatchEmitter Instance { get; } = new();

    static WhenDispatchEmitter() { }

    private WhenDispatchEmitter() { }

    protected override void WriteFileContent(CodeWriter cw, AggregateModel model) {
        using (cw.Block($"{model.Class.Accessibility} partial class {model.Class.Name} {{")) {
            using (cw.Block($"protected override void {EventSourcingConstants.AggregateRootClass.WhenMethodName}(global::{EventSourcingConstants.FQN.EventInterface} @event) {{")) {
                using (cw.Block("switch (@event) {")) {
                    foreach (var method in model.ApplyMethods) {
                        cw.WriteLine($"case {method.EventTypeFullName} typed:");
                        cw.Indent();
                        cw.WriteLine($"{method.MethodName}(typed);");
                        cw.WriteLine("return;");
                        cw.Dedent();
                    }
                }
            }
        }
    }
}
