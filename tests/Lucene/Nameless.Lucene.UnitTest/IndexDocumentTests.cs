namespace Nameless.Lucene;

public class IndexDocumentTests {
    [Test]
    public void WhenCreating_ThenThereShouldBeAFieldCalledID() {
        // arrange
        const string id = "123";
        var sut = new IndexDocument(id);

        // act
        var field = sut.FirstOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.That(field, Is.Not.Null);
            Assert.That(field.Name, Is.EqualTo(nameof(ISearchHit.IndexDocumentID)));
            Assert.That(field.Value, Is.EqualTo(id));
            Assert.That(field.Type, Is.EqualTo(IndexableType.Text));
            Assert.That(field.Options, Is.EqualTo(FieldOptions.Store));
        });
    }

    [Test]
    public void WhenSetDateTimeOffsetField_ThenStoreTheCorrectFieldAndMetadata() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        var value = new DateTimeOffset(year: 2000,
                                       month: 1,
                                       day: 1,
                                       hour: 12,
                                       minute: 30,
                                       second: 0,
                                       offset: TimeSpan.Zero);

        var sut = new IndexDocument(id);

        foreach (var option in Enum.GetValues<FieldOptions>()) {
            // act
            var field = sut.Set(fieldName, value, option).LastOrDefault();

            // assert
            Assert.Multiple(() => {
                Assert.That(field, Is.Not.Null);
                Assert.That(field.Name, Is.EqualTo(fieldName));
                Assert.That(field.Value, Is.EqualTo(value));
                Assert.That(field.Type, Is.EqualTo(IndexableType.DateTime));
                Assert.That(field.Options, Is.EqualTo(option));
            });
        }
    }

    [Test]
    public void WhenSetStringField_ThenStoreTheCorrectFieldAndMetadata() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        const string value = "this is a test";

        var sut = new IndexDocument(id);

        foreach (var option in Enum.GetValues<FieldOptions>()) {
            // act
            var field = sut.Set(fieldName, value, option).LastOrDefault();

            // assert
            Assert.Multiple(() => {
                Assert.That(field, Is.Not.Null);
                Assert.That(field.Name, Is.EqualTo(fieldName));
                Assert.That(field.Value, Is.EqualTo(value));
                Assert.That(field.Type, Is.EqualTo(IndexableType.Text));
                Assert.That(field.Options, Is.EqualTo(option));
            });
        }
    }

    [Test]
    public void WhenSetBooleanField_ThenStoreTheCorrectFieldAndMetadata() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        const bool value = true;

        var sut = new IndexDocument(id);

        foreach (var option in Enum.GetValues<FieldOptions>()) {
            // act
            var field = sut.Set(fieldName, value, option).LastOrDefault();

            // assert
            Assert.Multiple(() => {
                Assert.That(field, Is.Not.Null);
                Assert.That(field.Name, Is.EqualTo(fieldName));
                Assert.That(field.Value, Is.EqualTo(value));
                Assert.That(field.Type, Is.EqualTo(IndexableType.Boolean));
                Assert.That(field.Options, Is.EqualTo(option));
            });
        }
    }

    [Test]
    public void WhenSetDoubleField_ThenStoreTheCorrectFieldAndMetadata() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        const double value = 987.654D;

        var sut = new IndexDocument(id);

        foreach (var option in Enum.GetValues<FieldOptions>()) {
            // act
            var field = sut.Set(fieldName, value, option).LastOrDefault();

            // assert
            Assert.Multiple(() => {
                Assert.That(field, Is.Not.Null);
                Assert.That(field.Name, Is.EqualTo(fieldName));
                Assert.That(field.Value, Is.EqualTo(value));
                Assert.That(field.Type, Is.EqualTo(IndexableType.Number));
                Assert.That(field.Options, Is.EqualTo(option));
            });
        }
    }

    [Test]
    public void WhenSetIntegerField_ThenStoreTheCorrectFieldAndMetadata() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        const int value = 10001;

        var sut = new IndexDocument(id);

        foreach (var option in Enum.GetValues<FieldOptions>()) {
            // act
            var field = sut.Set(fieldName, value, option).LastOrDefault();

            // assert
            Assert.Multiple(() => {
                Assert.That(field, Is.Not.Null);
                Assert.That(field.Name, Is.EqualTo(fieldName));
                Assert.That(field.Value, Is.EqualTo(value));
                Assert.That(field.Type, Is.EqualTo(IndexableType.Integer));
                Assert.That(field.Options, Is.EqualTo(option));
            });
        }
    }

    [Test]
    public void WhenSetField_ShouldSetOptionsAsFlag() {
        // arrange
        const string id = "123";
        const string fieldName = "Field";
        const int value = 5000;
        const FieldOptions options = FieldOptions.Store | FieldOptions.Analyze;

        var sut = new IndexDocument(id);

        // act
        var field = sut.Set(fieldName, value, options).LastOrDefault();

        // assert
        Assert.Multiple(() => {
            Assert.That(field, Is.Not.Null);
            Assert.That(field.Name, Is.EqualTo(fieldName));
            Assert.That(field.Value, Is.EqualTo(value));
            Assert.That(field.Type, Is.EqualTo(IndexableType.Integer));
            Assert.That(field.Options.HasFlag(FieldOptions.Store));
            Assert.That(field.Options.HasFlag(FieldOptions.Analyze));
        });
    }
}
