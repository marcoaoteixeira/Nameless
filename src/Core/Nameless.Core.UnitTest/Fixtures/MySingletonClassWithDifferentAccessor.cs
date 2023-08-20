namespace Nameless.Core.UnitTests.Fixtures {

    [Singleton(AccessorName = "Default")]
    public sealed class MySingletonClassWithDifferentAccessor {

        private static readonly MySingletonClassWithDifferentAccessor _instance = new();

        public static MySingletonClassWithDifferentAccessor Default => _instance;

        private MySingletonClassWithDifferentAccessor() { }

        static MySingletonClassWithDifferentAccessor() { }
    }
}
