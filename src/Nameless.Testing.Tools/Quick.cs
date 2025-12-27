namespace Nameless.Testing.Tools;

public static class Quick {
    public static T Mock<T>() where T : class {
        return Moq.Mock.Of<T>();
    }
}