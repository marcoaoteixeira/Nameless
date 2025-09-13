namespace Nameless.Search;

public class SampleValueIndexableTypeInlineData : TheoryData<object, IndexableType> {
    public SampleValueIndexableTypeInlineData() {
        Add(p1: true, IndexableType.Boolean);
        Add(p1: "String Value", IndexableType.String);
        Add((byte)123, IndexableType.Byte);
        Add((short)456, IndexableType.Short);
        Add(p1: 789, IndexableType.Integer);
        Add(p1: 987L, IndexableType.Long);
        Add(p1: 654F, IndexableType.Float);
        Add(p1: 321D, IndexableType.Double);
        Add(DateTimeOffset.Now, IndexableType.DateTimeOffset);
        Add(DateTime.Now, IndexableType.DateTime);
    }
}