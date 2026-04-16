using System.Windows.Input;

namespace Nameless.WPF.Infrastructure;

/// <summary>
///     Represents an empty command that does nothing and cannot be executed.
/// </summary>
public sealed class EmptyCommand : ICommand {
    /// <summary>
    ///     The singleton instance of the <see cref="EmptyCommand"/> class.
    /// </summary>
    public static ICommand Instance { get; } = new EmptyCommand();

    static EmptyCommand() { }

    private EmptyCommand() { }

    /// <inheritdoc />
    public bool CanExecute(object? parameter) {
        return false;
    }

    /// <inheritdoc />
    public void Execute(object? parameter) { }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged {
        add { }
        remove { }
    }
}
