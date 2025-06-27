namespace Nameless.Web.Correlation;

/// <summary>
///     Correlation ID accessor interface.
/// </summary>
public interface ICorrelationAccessor {
    /// <summary>
    ///     Retrieves the correlation ID from the HTTP context.
    /// </summary>
    /// <returns>
    ///     A string representing the correlation ID.
    /// </returns>
    string? GetID();
}