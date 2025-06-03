namespace Nameless.Testing.Tools;

public static class Fast {
    public static T Mock<T>()
        where T : class {
        return Moq.Mock.Of<T>();
    }
}
