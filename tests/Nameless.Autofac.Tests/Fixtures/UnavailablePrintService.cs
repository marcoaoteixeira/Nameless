namespace Nameless.Autofac.Fixtures;

public class UnavailablePrintService : IPrintService {
    private readonly TextWriter _output;

    public UnavailablePrintService(TextWriter output) {
        _output = output;
    }

    public void Print(string value) {
        _output.WriteLine("Print service unavailable.");
    }
}