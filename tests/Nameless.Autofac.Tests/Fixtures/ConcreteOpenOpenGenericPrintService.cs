namespace Nameless.Autofac.Fixtures;

public class ConcreteOpenOpenGenericPrintService : IOpenGenericPrintService<DateTime> {
    private readonly TextWriter _output;

    public ConcreteOpenOpenGenericPrintService(TextWriter output) {
        _output = output;
    }

    public virtual void Print(DateTime value) {
        _output.WriteLine(value);
    }
}