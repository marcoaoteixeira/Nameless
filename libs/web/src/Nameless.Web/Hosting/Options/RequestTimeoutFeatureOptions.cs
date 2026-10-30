namespace Nameless.Web.Hosting.Options;

/// <summary>
///     Output Cache Feature Options.
/// </summary>
public sealed class RequestTimeoutFeatureOptions {
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
    ///     The current <see cref="RequestTimeoutFeatureOptions"/> instace
    ///     so other actions can be chained.
    /// </returns>
    public RequestTimeoutFeatureOptions WithIncludeDefaultPolicies(bool value) {
        IncludeDefaultPolicies = value;

        return this;
    }
}