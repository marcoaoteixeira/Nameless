using System.Reflection;
using Nameless.WPF.Resources;
using Wpf.Ui.Controls;

namespace Nameless.WPF.Navigation;

public class AutoDiscoverableNavigationViewItemProvider : INavigationViewItemProvider {
    private readonly Assembly[] _assemblies;
    private readonly Lazy<Entry[]> _entries;

    public AutoDiscoverableNavigationViewItemProvider(IEnumerable<Assembly> assemblies) {
        _assemblies = [.. assemblies];
        _entries = new Lazy<Entry[]>(DiscoverNavigationViewItems);
    }

    public IEnumerable<NavigationViewItem> GetMainItems() {
        return _entries.Value
                       .Where(entry => !entry.Attribute.Footer)
                       .Select(entry => entry.ViewIem);
    }

    public IEnumerable<NavigationViewItem> GetFooterItems() {
        return _entries.Value
                       .Where(entry => entry.Attribute.Footer)
                       .Select(entry => entry.ViewIem);
    }

    private Entry[] DiscoverNavigationViewItems() {
        return _assemblies.SelectMany(assembly => assembly.GetExportedTypes())
                          .Where(type => type.HasAttribute<NavigationViewItemAttribute>())
                          .Select(CreateTuple)
                          .Select(CreateEntry)
                          .ToArray();

        static Tuple<Type, NavigationViewItemAttribute> CreateTuple(Type type) {
            var attribute = type.GetCustomAttribute<NavigationViewItemAttribute>()
                            ?? throw new InvalidOperationException(string.Format(Strings.DiscoverableNavigationViewItemProvider_CreateTuple_Missing_Attribute_Exception, nameof(NavigationViewItemAttribute)));

            return new Tuple<Type, NavigationViewItemAttribute>(type, attribute);
        }

        static Entry CreateEntry(Tuple<Type, NavigationViewItemAttribute> tuple) {
            var (type, attribute) = tuple;
            var viewItem = new NavigationViewItem {
                Content = attribute.Title,
                Icon = new SymbolIcon { Symbol = attribute.Icon },
                TargetPageType = type,
                ToolTip = attribute.ToolTip
            };

            return new Entry(viewItem, attribute);
        }
    }

    internal record Entry(NavigationViewItem ViewIem, NavigationViewItemAttribute Attribute);
}