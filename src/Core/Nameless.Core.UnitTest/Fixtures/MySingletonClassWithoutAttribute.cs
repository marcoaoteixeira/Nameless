namespace Nameless.Core.UnitTests.Fixtures {

    public sealed class MySingletonClassWithoutAttribute {

        private static readonly MySingletonClassWithoutAttribute _instance = new();

        public static MySingletonClassWithoutAttribute Instance => _instance;

        private MySingletonClassWithoutAttribute() { }

        static MySingletonClassWithoutAttribute() { }
    }
}
