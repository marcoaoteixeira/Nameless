using Xunit.v3;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     Bug attribute for marking tests that are related to a specific issue or bug.
/// </summary>
/// <remarks>
///     This attribute can be applied to assemblies, classes, or methods to
///     indicate that they are bug test cases.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class BugAttribute : Attribute, ITraitAttribute {
    public string Issue { get; }
    /// <summary>
    ///     Gets or sets the author of the test.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BugAttribute"/> class.
    /// </summary>
    /// <param name="issue">
    ///     The issue identifier, typically a bug or task number.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="issue"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="issue"/> is an empty string or
    ///     contains only whitespace.
    /// </exception>
    public BugAttribute(string issue) {
        Prevent.Argument.NullOrWhiteSpace(issue);

        Issue = issue;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() {
        return [
            new KeyValuePair<string, string>("Category", "Bug"),
            new KeyValuePair<string, string>(nameof(Author), Author),
            new KeyValuePair<string, string>(nameof(Issue), Issue),
        ];
    }
}