using Wpf.Ui.Controls;

namespace Nameless.WPF.Navigation;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NavigationViewItemAttribute : Attribute {
    public required SymbolRegular Icon { get; init; }
    public required string Title { get; init; }
    public string? ToolTip { get; init; }
    public int Position { get; init; }
    public bool Footer { get; init; }
}
