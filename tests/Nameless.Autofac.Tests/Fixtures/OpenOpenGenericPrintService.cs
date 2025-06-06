namespace Nameless.Autofac.Fixtures;

public class OpenOpenGenericPrintService<T> : IOpenGenericPrintService<T> {
    protected TextWriter Output { get; }

    public OpenOpenGenericPrintService(TextWriter output) {
        Output = output;
    }

    public virtual void Print(T value) {
        Output.WriteLine(value);
    }
}