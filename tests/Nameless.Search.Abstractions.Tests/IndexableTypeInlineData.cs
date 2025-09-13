namespace Nameless.Search;

public sealed class IndexableTypeInlineData : TheoryData<IndexableType> {
    public IndexableTypeInlineData() {
        foreach (var indexableType in Enum.GetValues<IndexableType>()) {
            Add(indexableType);
        }
    }
}