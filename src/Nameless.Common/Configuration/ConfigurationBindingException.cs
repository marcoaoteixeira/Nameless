namespace Nameless.Configuration;

/// <summary>
///     Configuration binding exception.
/// </summary>
public class ConfigurationBindingException : Exception {
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
    public ConfigurationBindingException(string path, Type bindingType)
        : base($"Unable to bind section '{path}' to type '{bindingType}'.", innerException: null) { }
}
