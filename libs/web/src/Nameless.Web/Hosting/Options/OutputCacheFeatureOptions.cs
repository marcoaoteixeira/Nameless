namespace Nameless.Web.Hosting.Options;

/// <summary>
///     Output Cache Feature Options.
/// </summary>
public sealed class OutputCacheFeatureOptions {
    /// <summary>
    ///     Whether it should add the default policies.
    /// </summary>
    public bool IncludeDefaultPolicies { get; private set; }

    /// <summary>
    ///     Defines whether it should include the default policies when registering.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="OutputCacheFeatureOptions"/> instace
    ///     so other actions can be chained.
    /// </returns>
    public OutputCacheFeatureOptions WithIncludeDefaultPolicies(bool value) {
        IncludeDefaultPolicies = value;

        return this;
    }
}