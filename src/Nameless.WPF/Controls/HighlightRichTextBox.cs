using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Nameless.WPF.Controls;

/// <summary>
///     A <see cref="RichTextBox"/> that can highlight its content.
/// </summary>
public class HighlightRichTextBox : RichTextBox {
    private static readonly SolidColorBrush HighlightBrush = new(Colors.Yellow);

    /// <summary>
    ///     Dependency property for <see cref="Source"/>.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source),
                                    typeof(string),
                                    typeof(HighlightRichTextBox),
                                    new PropertyMetadata(SourceChangeHandler));

    /// <summary>
    ///     Dependency property for <see cref="HighlightTerms"/>.
    /// </summary>
    public static readonly DependencyProperty HighlightTermsProperty =
        DependencyProperty.Register(nameof(HighlightTerms),
                                    typeof(string[]),
                                    typeof(HighlightRichTextBox),
                                    new PropertyMetadata(HighlightChangeHandler));

    /// <summary>
    ///     Gets or sets the source.
    /// </summary>
    public string Source {
        get => GetValue(SourceProperty) as string ?? string.Empty;
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    ///     Gets or sets the terms to highlight.
    /// </summary>
    public string[] HighlightTerms {
        get => GetValue(HighlightTermsProperty) as string[] ?? [];
        set => SetValue(HighlightTermsProperty, value);
    }

    /// <summary>
    ///     Applies the highlight for the specified terms.
    /// </summary>
    /// <param name="terms">
    ///     The terms to highlight.
    /// </param>
    public void ApplyHighlight(string[] terms) {
        if (terms.Length == 0) { return; }

        var textRange = new TextRange(Document.ContentStart, Document.ContentEnd);

        textRange.ClearAllProperties();

        var content = textRange.Text;

        foreach (var term in terms) {
            if (!Regex.IsMatch(content, term, RegexOptions.IgnoreCase)) { continue; }

            var startPointer = Document.ContentStart;
            while (startPointer is not null) {
                if (startPointer.CompareTo(Document.ContentEnd) == 0) {
                    break;
                }

                var value = startPointer.GetTextInRun(LogicalDirection.Forward);
                var highlightTermPosition = value.IndexOf(term, StringComparison.CurrentCultureIgnoreCase);

                // Didn't find the highlight text in this Run, skip to next one.
                if (highlightTermPosition < 0) {
                    startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);
                    continue;
                }

                // Setting up the pointer here at this matched index
                startPointer = startPointer.GetPositionAtOffset(highlightTermPosition);

                if (startPointer is null) { break; }

                var nextPointer = startPointer.GetPositionAtOffset(term.Length);
                var searchedTextRange = new TextRange(startPointer, nextPointer);

                // Finally, highlight it
                searchedTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, HighlightBrush);

                // Move next
                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }

    private static void SourceChangeHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
        if (dependencyObject is not HighlightRichTextBox richTextBox) { return; }

        var range = new TextRange(
            richTextBox.Document.ContentStart,
            richTextBox.Document.ContentEnd
        );
        var buffer = Encoding.UTF8.GetBytes(richTextBox.Source);

        using var stream = new MemoryStream(buffer);

        range.Load(stream, DataFormats.Text);

        richTextBox.ApplyHighlight(richTextBox.HighlightTerms);
    }

    private static void HighlightChangeHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
        if (dependencyObject is not HighlightRichTextBox richTextBox) { return; }

        richTextBox.ApplyHighlight(richTextBox.HighlightTerms);
    }
}
