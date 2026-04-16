using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Nameless.WPF.Controls;

/// <summary>
///     A <see cref="RichTextBox"/> that displays log entries with different styles.
/// </summary>
public class LoggerRichTextBox : RichTextBox {
    /// <summary>
    ///     Dependency property for <see cref="Entries"/>.
    /// </summary>
    public static readonly DependencyProperty EntriesProperty =
        DependencyProperty.Register(
            nameof(Entries),
            typeof(ObservableCollection<LoggerRichTextBoxEntry>),
            typeof(LoggerRichTextBox),
            new PropertyMetadata(EntriesChangeHandler)
        );

    /// <summary>
    ///     Gets or sets the log entries to display.
    /// </summary>
    public ObservableCollection<LoggerRichTextBoxEntry> Entries {
        get => (ObservableCollection<LoggerRichTextBoxEntry>)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    /// <summary>
    ///     Initializes a new instance of
    ///     the <see cref="LoggerRichTextBox"/> class.
    /// </summary>
    public LoggerRichTextBox() {
        IsReadOnly = true; // Prevent user editing
        IsReadOnlyCaretVisible = false; // Optional: hide caret
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        Background = Brushes.Transparent;
        Foreground = Brushes.Black;
        FontFamily = new FontFamily("Consolas"); // Monospaced look like logs
        FontSize = 13;
    }

    private static void EntriesChangeHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
        if (dependencyObject is not LoggerRichTextBox control) {
            return;
        }

        if (args.OldValue is ObservableCollection<LoggerRichTextBoxEntry> oldEntries) {
            oldEntries.CollectionChanged -= control.OnCollectionChangeHandler;
        }

        if (args.NewValue is ObservableCollection<LoggerRichTextBoxEntry> newEntries) {
            newEntries.CollectionChanged += control.OnCollectionChangeHandler;
        }

        control.Refresh();
    }

    private void OnCollectionChangeHandler(object? sender, NotifyCollectionChangedEventArgs args) {
        if (!Dispatcher.CheckAccess()) {
            Dispatcher.Invoke(() => OnCollectionChangeHandler(sender, args));

            return;
        }

        if (args.NewItems is not null) {
            foreach (LoggerRichTextBoxEntry entry in args.NewItems) {
                InnerWriteLine(entry);
            }
        }

        if (args.Action == NotifyCollectionChangedAction.Reset) {
            Document.Blocks.Clear();
        }
    }

    private void InnerWriteLine(LoggerRichTextBoxEntry entry) {
        if (Document.Blocks.LastBlock is not Paragraph paragraph) {
            paragraph = new Paragraph();
            Document.Blocks.Add(paragraph);
        }

        var run = new Run($"[{DateTime.Now:HH:mm:ss}] {entry.Message}{Environment.NewLine}") {
            Foreground = entry.Foreground,
            Background = entry.Background
        };

        paragraph.Inlines.Add(run);

        // Auto scroll to end
        CaretPosition = Document.ContentEnd;

        ScrollToEnd();
    }

    private void Refresh() {
        Document.Blocks.Clear();

        if (Entries.Count == 0) { return; }

        foreach (var entry in Entries) {
            InnerWriteLine(entry);
        }
    }
}