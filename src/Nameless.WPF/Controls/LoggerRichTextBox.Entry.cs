using System.Windows.Media;

namespace Nameless.WPF.Controls;

/// <summary>
///     Represents a log entry for the <see cref="LoggerRichTextBox"/>.
/// </summary>
public record LoggerRichTextBoxEntry {
    /// <summary>
    ///     Gets or init the log message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or init the background brush.
    /// </summary>
    public Brush Background { get; init; } = Brushes.Transparent;

    /// <summary>
    ///     Gets or init the foreground brush.
    /// </summary>
    public Brush Foreground { get; init; } = Brushes.Black;

    // Private constructor to enforce the use of static methods for
    // creating instances.
    private LoggerRichTextBoxEntry() { }

    /// <summary>
    ///     Creates an information log entry.
    /// </summary>
    /// <param name="message">
    ///     The message
    /// </param>
    /// <returns>
    ///     An instance of <see cref="LoggerRichTextBoxEntry"/>
    ///     representing an information log entry.
    /// </returns>
    public static LoggerRichTextBoxEntry Information(string message) {
        return new LoggerRichTextBoxEntry {
            Message = message,
            Background = Brushes.Transparent,
            Foreground = Brushes.Black
        };
    }

    /// <summary>
    ///     Creates an error log entry.
    /// </summary>
    /// <param name="message">
    ///     The message
    /// </param>
    /// <returns>
    ///     An instance of <see cref="LoggerRichTextBoxEntry"/>
    ///     representing an error log entry.
    /// </returns>
    public static LoggerRichTextBoxEntry Error(string message) {
        return new LoggerRichTextBoxEntry {
            Message = message,
            Background = Brushes.DarkSalmon,
            Foreground = Brushes.DarkRed
        };
    }

    /// <summary>
    ///     Creates a success log entry.
    /// </summary>
    /// <param name="message">
    ///     The message
    /// </param>
    /// <returns>
    ///     An instance of <see cref="LoggerRichTextBoxEntry"/>
    ///     representing a success log entry.
    /// </returns>
    public static LoggerRichTextBoxEntry Success(string message) {
        return new LoggerRichTextBoxEntry {
            Message = message,
            Background = Brushes.Transparent,
            Foreground = Brushes.Green
        };
    }

    /// <summary>
    ///     Creates a warning log entry.
    /// </summary>
    /// <param name="message">
    ///     The message
    /// </param>
    /// <returns>
    ///     An instance of <see cref="LoggerRichTextBoxEntry"/>
    ///     representing a warning log entry.
    /// </returns>
    public static LoggerRichTextBoxEntry Warning(string message) {
        return new LoggerRichTextBoxEntry {
            Message = message,
            Background = Brushes.Transparent,
            Foreground = Brushes.Goldenrod
        };
    }
}