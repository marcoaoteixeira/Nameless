namespace Nameless.Helpers;

/// <summary>
///     A simple delegate executor to help construct objects and configure them
///     given the specified action. Really, nothing fancy.
/// </summary>
public static class ActionHelper {
    /// <summary>
    ///     Creates an instance of the <typeparamref name="T"/> class and
    ///     executes the given delegate over it.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the class.
    /// </typeparam>
    /// <param name="action">
    ///     The delegate.
    /// </param>
    /// <returns>
    ///     The <typeparamref name="T"/> instance.
    /// </returns>
    public static T FromDelegate<T>(Action<T>? action)
        where T : class, new() {
        var innerAction = action ?? (_ => { });
        var result = new T();

        innerAction(result);

        return result;
    }
}
