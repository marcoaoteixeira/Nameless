using Moq;

namespace Nameless.Testing.Tools;

public static class Quick {
    public static T Mock<T>(bool strict = false) where T : class {
        return Moq.Mock.Of<T>(strict ? MockBehavior.Strict : MockBehavior.Loose);
    }
}