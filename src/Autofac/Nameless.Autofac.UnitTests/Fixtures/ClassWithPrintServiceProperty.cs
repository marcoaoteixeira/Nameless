namespace Nameless.Autofac.Fixtures;

public class ClassWithPrintServiceProperty {
    public IPrintService PrintService { get; set; }

    public void Write(string text)
        => PrintService?.Print(text);
}
