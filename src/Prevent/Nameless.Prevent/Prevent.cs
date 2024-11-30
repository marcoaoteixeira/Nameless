namespace Nameless;

/// <summary>
/// A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
/// is a software pattern that simplifies complex functions by
/// "failing fast", checking for invalid inputs up front and immediately
/// failing if any are found.
/// </summary>
public sealed class Prevent {
    /// <summary>
    /// Gets the unique instance of <see cref="Prevent" />.
    /// </summary>
    public static Prevent Argument { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Prevent() { }

    private Prevent() { }
}