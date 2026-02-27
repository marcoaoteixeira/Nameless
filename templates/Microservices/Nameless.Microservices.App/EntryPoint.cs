using Nameless.Microservices.StartUp;

namespace Nameless.Microservices.App;

public class EntryPoint {
    public static void Main(string[] args) {
        WebApp.Create(settings => settings.Args = args)
              .Run();
    }
}
