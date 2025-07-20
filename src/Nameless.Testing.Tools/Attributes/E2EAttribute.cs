using Xunit.v3;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     End-to-end tests are tests that verify the entire system from end to
///     end. They typically require the most setup and teardown and most likely
///     will depend on external resources or services. They are typically the
///     slowest tests and should be run infrequently.
/// </summary>
/// <remarks>
///     This attribute can be applied to assemblies, classes, or methods to
///     indicate that they are end-to-end tests.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class E2EAttribute : Attribute, ITraitAttribute {
    /// <summary>
    ///     Gets or sets the author of the test.
    /// </summary>
    public string? Author { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() {
        return [
            new KeyValuePair<string, string>(nameof(Category), nameof(Category.E2E)),
            new KeyValuePair<string, string>(nameof(Author), Author ?? string.Empty)
        ];
    }
}