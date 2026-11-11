using System.Collections.Concurrent;
using Nameless.Contracts;

namespace Nameless.Windows.Localization;

/// <summary>
///     Default <see cref="ITranslatorFactory"/> implementation. Wraps one
///     shared root <see cref="ITranslator"/> and caches one
///     <see cref="SectionTranslator"/> instance per section name, so
///     repeated resolution for the same section (or the same anchor type)
///     always returns the same instance and never leaks a new subscription
///     on the root.
/// </summary>
public sealed class TranslatorFactory : ITranslatorFactory {
    private readonly ITranslator _root;
    private readonly ConcurrentDictionary<string, ITranslator> _cache = new(StringComparer.Ordinal);

    /// <summary>
    ///     Initializes a new instance of <see cref="TranslatorFactory"/>
    ///     class.
    /// </summary>
    /// <param name="root">
    ///     The root translator every resolved section-scoped translator
    ///     delegates to.
    /// </param>
    public TranslatorFactory(ITranslator root) {
        _root = Throws.When.Null(root);
    }

    /// <inheritdoc />
    public ITranslator Create(Type anchor) {
        return Create(TranslationKey.GetSection(anchor));
    }

    /// <inheritdoc />
    public ITranslator Create(string section) {
        Throws.When.NullOrWhiteSpace(section);

        return _cache.GetOrAdd(section, static (key, root) => new SectionTranslator(root, key), _root);
    }
}
