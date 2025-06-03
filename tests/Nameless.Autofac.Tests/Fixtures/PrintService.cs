namespace Nameless.Autofac.Fixtures;

public class PrintService : IPrintService {
    private readonly TextWriter _output;

    public PrintService(TextWriter output) {
        _output = output;
    }

    public void Print(string value) {
        _output.WriteLine(value);
    }
}