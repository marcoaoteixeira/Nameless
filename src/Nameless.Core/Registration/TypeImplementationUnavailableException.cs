namespace Nameless.Registration;

public class TypeImplementationUnavailableException(Type type)
    : Exception($"Unable to locate a suitable implementation for '{type.GetPrettyName()}'.");
