using System.Reflection;
using Nameless.Windows.Localization;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Auto Discoverable Navigation View Item Provider
/// </summary>
public class AutoDiscoverableNavigationViewItemProvider : INavigationViewItemProvider {
    private readonly Assembly[] _assemblies;
    private readonly ITranslator? _translator;
    private readonly Lazy<Entry[]> _entries;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AutoDiscoverableNavigationViewItemProvider"/> class.
    /// </summary>
    /// <param name="assemblies">
    ///     A collection of assemblies.
    /// </param>
    /// <param name="translator">
    ///     The translator used to resolve <see cref="NavigationViewItemAttribute.TitleResourceName"/>
    ///     and <see cref="NavigationViewItemAttribute.ToolTipResourceName"/>
    ///     when no <see cref="NavigationViewItemAttribute.ResourceType"/> is
    ///     set. <see langword="null"/> falls back to the attribute's own
    ///     resx-based resolution.
    /// </param>
    public AutoDiscoverableNavigationViewItemProvider(IEnumerable<Assembly> assemblies, ITranslator? translator = null) {
        _assemblies = [.. assemblies];
        _translator = translator;
        _entries = new Lazy<Entry[]>(CreateEntries);
    }

    // Test-only seam: proves NavigationHostFeature actually wires the
    // container's ITranslator into this provider, without forcing an
    // STA thread (Build()/GetMenuItems() construct real WPF controls).
    internal ITranslator? Translator => _translator;

    /// <inheritdoc />
    public IEnumerable<NavigationViewItem> GetMenuItems() {
        return _entries.Value
                       .Where(entry => entry.Attribute.Placement == Placement.Menu)
                       .Select(entry => entry.Build(_translator));
    }

    /// <inheritdoc />
    public IEnumerable<NavigationViewItem> GetFooterItems() {
        return _entries.Value
                       .Where(entry => entry.Attribute.Placement == Placement.Footer)
                       .Select(entry => entry.Build(_translator));
    }

    private Entry[] CreateEntries() {
        return [
            .. _assemblies.SelectMany(assembly => assembly.GetTypes())
                          .Where(type => type.HasAttribute<NavigationViewItemAttribute>())
                          .Select(CreateEntry)
        ];

        static Entry CreateEntry(Type type) {
            var attribute = type.GetCustomAttribute<NavigationViewItemAttribute>()
                            ?? throw new InvalidOperationException(
                                $"Type is missing attribute '{nameof(NavigationViewItemAttribute)}'."
                            );

            return new Entry(type, attribute);
        }
    }

    internal record Entry(Type TargetPage, NavigationViewItemAttribute Attribute) {
        public NavigationViewItem Build(ITranslator? translator) {
            return new NavigationViewItem {
                Content = ResolveTitle(translator),
                Icon = new SymbolIcon { Symbol = Attribute.Icon },
                TargetPageType = TargetPage,
                ToolTip = ResolveToolTip(translator)
            };
        }

        // No ResourceType means the attribute has nothing resx-backed to
        // fall back to for this key — route it through the live
        // ITranslator instead, so it tracks culture switches the same way
        // the resx path used to (re-evaluated on every call, same as
        // Attribute.Title/ToolTip below).
        internal string ResolveTitle(ITranslator? translator) {
            return translator is not null &&
                   Attribute.ResourceType is null &&
                   !string.IsNullOrWhiteSpace(Attribute.TitleResourceName)
                ? translator[Attribute.TitleResourceName]
                : Attribute.Title;
        }

        internal string? ResolveToolTip(ITranslator? translator) {
            return translator is not null &&
                   Attribute.ResourceType is null &&
                   !string.IsNullOrWhiteSpace(Attribute.ToolTipResourceName)
                ? translator[Attribute.ToolTipResourceName]
                : Attribute.ToolTip;
        }
    }
}
