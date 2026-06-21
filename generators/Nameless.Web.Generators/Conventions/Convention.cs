namespace Nameless.Web.Generators.Conventions;

/// <summary>
///     One fluent call to chain on the route handler builder.
///     Stored as a literal fragment that follows the "builder",
///     e.g.: <c>RequireCors("cors-policy")</c>.
/// </summary>
public readonly record struct Convention {
    /// <summary>
    ///     Get the fluent call.
    /// </summary>
    public string Call { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="Convention"/> class.
    /// </summary>
    /// <param name="call">
    ///     The fluent call.
    /// </param>
    public Convention(string call) {
        Call = call;
    }
}