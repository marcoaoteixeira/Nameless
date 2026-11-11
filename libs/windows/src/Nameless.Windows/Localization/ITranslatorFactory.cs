namespace Nameless.Windows.Localization;

/// <summary>
///     Resolves a section-scoped <see cref="ITranslator"/>, in the style of
///     <c>ILoggerFactory</c>. The default implementation,
///     <see cref="TranslatorFactory"/>, caches one instance per section.
/// </summary>
public interface ITranslatorFactory {
    /// <summary>
    ///     Resolves the <see cref="ITranslator"/> scoped to the section
    ///     mapped from <paramref name="anchor"/>. See
    ///     <see cref="TranslationKey.GetSection(Type)"/>.
    /// </summary>
    /// <param name="anchor">
    ///     The anchor type.
    /// </param>
    /// <returns>
    ///     The section-scoped <see cref="ITranslator"/>.
    /// </returns>
    ITranslator Create(Type anchor);

    /// <summary>
    ///     Resolves the <see cref="ITranslator"/> scoped to
    ///     <paramref name="section"/> explicitly, bypassing the
    ///     type-to-section convention.
    /// </summary>
    /// <param name="section">
    ///     The section name.
    /// </param>
    /// <returns>
    ///     The section-scoped <see cref="ITranslator"/>.
    /// </returns>
    ITranslator Create(string section);
}

/// <summary>
///     Extension methods for <see cref="ITranslatorFactory"/>.
/// </summary>
public static class TranslatorFactoryExtensions {
    /// <param name="self">
    ///     The current <see cref="ITranslatorFactory"/> instance.
    /// </param>
    extension(ITranslatorFactory self) {
        /// <summary>
        ///     Resolves the <see cref="ITranslator{T}"/> scoped to the
        ///     section mapped from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The anchor type.
        /// </typeparam>
        /// <returns>
        ///     The typed, section-scoped translator.
        /// </returns>
        public ITranslator<T> Create<T>() {
            return new Translator<T>(self);
        }
    }
}
