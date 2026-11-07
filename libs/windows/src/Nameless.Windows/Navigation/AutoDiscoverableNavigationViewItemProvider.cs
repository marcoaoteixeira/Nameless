using System.Reflection;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     Auto Discoverable Navigation View Item Provider
/// </summary>
public class AutoDiscoverableNavigationViewItemProvider : INavigationViewItemProvider {
    private readonly Assembly[] _assemblies;
    private readonly Lazy<Entry[]> _entries;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AutoDiscoverableNavigationViewItemProvider"/> class.
    /// </summary>
    /// <param name="assemblies">
    ///     A collection of assemblies.
    /// </param>
    public AutoDiscoverableNavigationViewItemProvider(IEnumerable<Assembly> assemblies) {
        _assemblies = [.. assemblies];
        _entries = new Lazy<Entry[]>(CreateEntries);
    }

    /// <inheritdoc />
    public IEnumerable<NavigationViewItem> GetMenuItems() {
        return _entries.Value
                       .Where(entry => entry.Attribute.Placement == Placement.Menu)
                       .Select(entry => entry.Build());
    }

    /// <inheritdoc />
    public IEnumerable<NavigationViewItem> GetFooterItems() {
        return _entries.Value
                       .Where(entry => entry.Attribute.Placement == Placement.Footer)
                       .Select(entry => entry.Build());
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
        public NavigationViewItem Build() {
            return new NavigationViewItem {
                Content = Attribute.Title,
                Icon = new SymbolIcon { Symbol = Attribute.Icon },
                TargetPageType = TargetPage,
                ToolTip = Attribute.ToolTip
            };
        }
    }
}
