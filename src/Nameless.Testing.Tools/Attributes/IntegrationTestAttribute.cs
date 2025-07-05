using Xunit.v3;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     Integration tests are tests that verify the interaction between
///     multiple components or systems. They typically require more setup and
///     teardown than unit tests and may depend on external resources or
///     services.
/// </summary>
/// <remarks>
///     This attribute can be applied to assemblies, classes, or methods to
///     indicate that they are integration tests.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class IntegrationTestAttribute : Attribute, ITraitAttribute {
    /// <summary>
    ///     Gets or sets the author of the test.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() {
        return [
            new KeyValuePair<string, string>("Category", "IntegrationTest"),
            new KeyValuePair<string, string>(nameof(Author), Author)
        ];
    }
}