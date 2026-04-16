using Wpf.Ui.Controls;

namespace Nameless.WPF.SnackBar;

public record SnackBarArgs {
    public required string Content { get; init; }
    public string? Title { get; init; }
    public ControlAppearance Appearance { get; init; }
}
