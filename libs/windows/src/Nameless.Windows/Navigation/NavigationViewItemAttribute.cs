using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     When applied to <see cref="INavigationView"/> defines a new
///     entry in the navigation menus.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NavigationViewItemAttribute : Attribute {
    /// <summary>
    ///     Gets or sets the icon.
    /// </summary>
    public required SymbolRegular Icon { get; init; }

    /// <summary>
    ///     Gets or sets the title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///     Gets or sets the tool tip.
    /// </summary>
    public string? ToolTip { get; init; }

    /// <summary>
    ///     Gets or sets the position.
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    ///     Whether it should be placed in the footer.
    /// </summary>
    public bool Footer { get; init; }
}
