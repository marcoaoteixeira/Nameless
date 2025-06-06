namespace Nameless.Autofac.Fixtures;

public class OverrideOpenOpenGenericPrintService<T> : OpenOpenGenericPrintService<T> {
    public OverrideOpenOpenGenericPrintService(TextWriter output)
        : base(output) { }

    public override void Print(T value) {
        Output.WriteLine($"{nameof(OverrideOpenOpenGenericPrintService<T>)}: {value}");
    }
}