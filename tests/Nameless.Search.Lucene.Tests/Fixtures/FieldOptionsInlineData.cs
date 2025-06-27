namespace Nameless.Search.Lucene.Fixtures;

public sealed class FieldOptionsInlineData : TheoryData<FieldOptions> {
    public FieldOptionsInlineData() {
        foreach (var fieldOption in Enum.GetValues<FieldOptions>()) {
            Add(fieldOption);
        }
    }
}
