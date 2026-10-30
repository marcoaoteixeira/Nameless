using Nameless.Contracts;

namespace Nameless.Configuration;

/// <summary>
///     Configuration binding exception.
/// </summary>
public class ConfigurationBindingException : Exception {
    /// <summary>
    ///     Gets the configuration path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     Gets the binding type.
    /// </summary>
    public Type BindingType { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ConfigurationBindingException"/> class.
    /// </summary>
    /// <param name="path">
    ///     Path to the section.
    /// </param>
    /// <param name="bindingType">
    ///     The binding type.
    /// </param>
    public ConfigurationBindingException(string path, Type bindingType) : base($"Unable to bind section '{path}' to type '{bindingType}'.", innerException: null) {
        Path = Throws.When.NullOrWhiteSpace(path);
        BindingType = Throws.When.Null(bindingType);
    }
}
