using Moq;

namespace Nameless.Testing.Tools;

/// <summary>
///     Quick mock creator.
/// </summary>
public static class Quick {
    /// <summary>
    ///     Quickly creates a mock for a given service.
    /// </summary>
    /// <typeparam name="T">
    ///     The service type.
    /// </typeparam>
    /// <param name="strict">
    ///     Whether it should be a strict mock.
    /// </param>
    /// <returns>
    ///     The mocked service.
    /// </returns>
    public static T Mock<T>(bool strict = false) where T : class {
        return Moq.Mock.Of<T>(strict ? MockBehavior.Strict : MockBehavior.Loose);
    }
}