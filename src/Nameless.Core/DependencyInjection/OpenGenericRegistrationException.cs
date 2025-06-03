namespace Nameless.DependencyInjection;

/// <summary>
/// Exception thrown when trying to register open generic types in the dependency injection container.
/// </summary>
public sealed class OpenGenericRegistrationException : Exception {
    private const string MESSAGE_FORMATTER = "Can't register open generics for the type '{0}'.";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericRegistrationException"/> class.
    /// </summary>
    /// <param name="openGenericType">The open generic type.</param>
    public OpenGenericRegistrationException(Type openGenericType)
        : base(string.Format(MESSAGE_FORMATTER, openGenericType.GetPrettyName())) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGenericRegistrationException"/> class.
    /// </summary>
    /// <param name="openGenericType">The open generic type.</param>
    /// <param name="inner">The inner exception.</param>
    public OpenGenericRegistrationException(Type openGenericType, Exception inner)
        : base(string.Format(MESSAGE_FORMATTER, openGenericType.GetPrettyName()), inner) { }
}
