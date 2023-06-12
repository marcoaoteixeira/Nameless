namespace Nameless.Core.UnitTests.Fixtures {

    [Singleton]
    public sealed class MySingletonClass {

        private static readonly MySingletonClass _instance = new();

        public static MySingletonClass Instance => _instance;

        private MySingletonClass() { }

        static MySingletonClass() { }
    }
}
