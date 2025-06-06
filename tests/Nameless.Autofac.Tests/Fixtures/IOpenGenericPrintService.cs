namespace Nameless.Autofac.Fixtures;

public interface IOpenGenericPrintService<in T> {
    void Print(T value);
}