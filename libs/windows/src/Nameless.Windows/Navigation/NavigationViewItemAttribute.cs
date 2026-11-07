using Nameless.Contracts;
using Nameless.Helpers;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

/// <summary>
///     When applied to <see cref="INavigationView"/> defines a new
///     entry in the navigation menus.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NavigationViewItemAttribute : Attribute {
    private string? _title;
    private string? _toolTip;

    /// <summary>
    ///     Gets or sets the icon.
    /// </summary>
    public required SymbolRegular Icon { get; init; }

    /// <summary>
    ///     Gets or sets the title.
    /// </summary>
    public string Title {
        get => GetTitle();
        init => SetTitle(value);
    }

    /// <summary>
    ///     Gets or sets the tool tip.
    /// </summary>
    public string? ToolTip {
        get => GetToolTip();
        init => SetToolTip(value);
    }

    /// <summary>
    ///     Gets or sets the position.
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    ///     Gets or sets the menu item placement
    /// </summary>
    public Placement Placement { get; init; }

    /// <summary>
    ///     Gets or sets the resources type.
    /// </summary>
    public Type? ResourceType { get; init; }

    /// <summary>
    ///     Gets or sets the name of the property in <see cref="ResourceType"/>
    ///     the will provide the title.
    /// </summary>
    public string? TitleResourceName { get; init; }

    /// <summary>
    ///     Gets or sets the name of the property in <see cref="ResourceType"/>
    ///     the will provide the tool tip.
    /// </summary>
    public string? ToolTipResourceName { get; init; }
    
    private string GetTitle() {
        if (!string.IsNullOrWhiteSpace(_title)) { return _title; }
        if (string.IsNullOrWhiteSpace(TitleResourceName)) {
            throw new InvalidOperationException(
                $"Must provide '{nameof(TitleResourceName)}'."
            );
        }
        if (ResourceType is null) { return TitleResourceName; }

        return ResourceManagerCache.GetString(ResourceType, TitleResourceName) ??
               TitleResourceName;
    }

    private void SetTitle(string value) {
        _title = Throws.When.NullOrWhiteSpace(value);
    }

    private string? GetToolTip() {
        if (!string.IsNullOrWhiteSpace(_toolTip)) { return _toolTip; }
        if (string.IsNullOrWhiteSpace(ToolTipResourceName)) { return null; }
        if (ResourceType is null) { return ToolTipResourceName; }
        
        return ResourceManagerCache.GetString(ResourceType, ToolTipResourceName) ??
               ToolTipResourceName;
    }

    private void SetToolTip(string? value) {
        _toolTip = value;
    }
}

/// <summary>
///     Navigation View Item Menu Placement
/// </summary>
public enum Placement {
    /// <summary>
    ///     In menu component
    /// </summary>
    Menu,

    /// <summary>
    ///     In footer component
    /// </summary>
    Footer
}
