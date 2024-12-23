namespace Nameless;

/// <summary>
/// Clock contract.
/// </summary>
public interface IClock {
    /// <summary>
    /// Retrieves UTC date time (now)
    /// </summary>
    /// <returns>
    /// <see cref="DateTime"/> UTC now.
    /// </returns>
    DateTime GetUtcNow();

    /// <summary>
    /// Retrieves UTC date time (now with offset)
    /// </summary>
    /// <returns>
    /// <see cref="DateTimeOffset"/> UTC now.
    /// </returns>
    DateTimeOffset GetUtcNowOffset();
}