namespace Nameless.Autofac.Fixtures;

public interface IPrintService {
    void Print(string value);
}

public class ConsolePrintService : IPrintService {
    public void Print(string value)
        => Console.WriteLine(value);
}

public class NoPrintService : IPrintService {
    public void Print(string value) { }
}

public interface IPrintServiceGeneric<in T> {
    void Print(T value);
}

public class ConsolePrintServiceGeneric<T> : IPrintServiceGeneric<T> {
    public virtual void Print(T value)
        => Console.WriteLine(value);
}

public class OverrideConsolePrintServiceGeneric<T> : ConsolePrintServiceGeneric<T> {
    public override void Print(T value)
        => Console.WriteLine(value);
}

public class ConcreteConsolePrintServiceGeneric : IPrintServiceGeneric<DateTime> {
    public virtual void Print(DateTime value)
        => Console.WriteLine(value);
}
