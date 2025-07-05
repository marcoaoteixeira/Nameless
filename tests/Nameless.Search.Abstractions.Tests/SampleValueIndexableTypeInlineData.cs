namespace Nameless.Search;
public class SampleValueIndexableTypeInlineData : TheoryData<object, IndexableType> {
    public SampleValueIndexableTypeInlineData() {
        Add(true, IndexableType.Boolean);
        Add("String Value", IndexableType.String);
        Add((byte)123, IndexableType.Byte);
        Add((short)456, IndexableType.Short);
        Add(789, IndexableType.Integer);
        Add(987L, IndexableType.Long);
        Add(654F, IndexableType.Float);
        Add(321D, IndexableType.Double);
        Add(DateTimeOffset.Now, IndexableType.DateTimeOffset);
        Add(DateTime.Now, IndexableType.DateTime);
    }
}
