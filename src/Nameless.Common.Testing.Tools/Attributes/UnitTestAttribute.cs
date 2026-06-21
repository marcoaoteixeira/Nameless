using Xunit.v3;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     Unit tests are tests that are run in isolation and do not depend on any
///     external resources or services. They are typically fast and should not
///     require any setup or teardown.
/// </summary>
/// <remarks>
///     This attribute can be applied to assemblies, classes, or methods to
///     indicate that they are unit tests.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UnitTestAttribute : Attribute, ITraitAttribute {
    /// <summary>
    ///     Gets or sets the author of the test.
    /// </summary>
    public string? Author { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() {
        return [
            new KeyValuePair<string, string>(nameof(Category), nameof(Category.UnitTest)),
            new KeyValuePair<string, string>(nameof(Author), Author ?? string.Empty)
        ];
    }
}