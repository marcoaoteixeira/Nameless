using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Fluent registration options for the translator subsystem.
/// </summary>
public class TranslatorFeatureOptions : AssemblyScanAware<TranslatorFeatureOptions> {
    private static readonly Type TranslatorType = typeof(ITranslator);

    /// <summary>
    ///     The concrete <see cref="ITranslator"/> type to register.
    ///     <see langword="null"/> when assembly scanning resolves the type
    ///     instead.
    /// </summary>
    public Type? Translator {
        get => UseAssemblyScan
            ? ExecuteAssemblyScan(TranslatorType).SingleOrDefault()
            : field;
        private set;
    }

    /// <summary>
    ///     Explicitly sets the translator implementation type.
    /// </summary>
    /// <typeparam name="TTranslator">
    ///     Type of the translator.
    /// </typeparam>
    /// <returns>
    ///     This instance for fluent chaining.
    /// </returns>
    public TranslatorFeatureOptions WithTranslator<TTranslator>() where TTranslator : ITranslator {
        return WithTranslator(typeof(TTranslator));
    }

    /// <summary>
    ///     Explicitly sets the <see cref="ITranslator"/> implementation type.
    ///     Overrides assembly scanning when called.
    /// </summary>
    /// <param name="type">
    ///     A concrete type that implements <see cref="ITranslator"/>.
    /// </param>
    /// <returns>
    ///     This instance for fluent chaining.
    /// </returns>
    public TranslatorFeatureOptions WithTranslator(Type type) {
        Throws.When.NonConcreteType(type);
        Throws.When.OpenGenericType(type);
        Throws.When.NonAssignableFrom(type, lhs: TranslatorType);

        Translator = type;

        return this;
    }
}