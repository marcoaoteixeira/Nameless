﻿namespace Nameless.Core.UnitTests.Fixtures {

    [Singleton]
    public sealed class MySingletonClass {

        public static MySingletonClass Instance { get; } = new();

        private MySingletonClass() { }

        static MySingletonClass() { }
    }
}
