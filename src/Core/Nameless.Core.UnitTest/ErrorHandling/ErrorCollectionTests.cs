namespace Nameless.ErrorHandling;
public class ErrorCollectionTests {
    [Test]
    public void When_Creating_Instance_With_No_Initial_Dictionary_Then_Returns_Empty_Collection() {
        // arrange
        
        // act
        var actual = new ErrorCollection();

        // assert
        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void When_Creating_Instance_With_Initial_Dictionary_Then_Returns_Collection_With_Items() {
        // arrange
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };

        // act
        var actual = new ErrorCollection(initial);

        // assert
        Assert.Multiple(() => {
            Assert.That(actual, Is.Not.Empty);
            Assert.That(actual, Has.Count.EqualTo(initial.Count));
        });
    }

    [Test]
    public void When_Adding_Entry_Then_Collection_Adds_New_Entry() {
        // arrange
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };

        // act
        var actual = new ErrorCollection(initial);
        actual.Add("Error Code 2", ["Message 1", "Message 2"]);

        // assert
        Assert.Multiple(() => {
            Assert.That(actual, Is.Not.Empty);
            Assert.That(actual, Has.Count.EqualTo(initial.Count + 1));
        });
    }

    [Test]
    public void When_Adding_Entry_With_Present_Error_Code_Then_Merge_Error_Problems() {
        // arrange
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };
        var expectedMessages = new[] { "Message 1", "Message 2", "Message 3" };

        // act

        // ReSharper disable once UseObjectOrCollectionInitializer
        var sut = new ErrorCollection(initial);
        sut.Add("Error Code", ["Message 2", "Message 3"]);

        // assert
        Assert.Multiple(() => {
            Assert.That(sut, Is.Not.Empty);
            Assert.That(sut, Has.Count.EqualTo(1));
            Assert.That(sut.First().Problems , Is.EquivalentTo(expectedMessages));
        });
    }

    // Since ErrorCollection implicit implements ICollection<Error>
    [Test]
    public void When_Adding_Entry_Error_With_Present_Error_Code_Then_Merge_Error_Problems() {
        // arrange
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };
        var expectedMessages = new[] { "Message 1", "Message 2", "Message 3" };

        // act
        ICollection<Error> sut = new ErrorCollection(initial);
        var error = new Error("Error Code", ["Message 2", "Message 3"]);
        sut.Add(error);

        // assert
        Assert.Multiple(() => {
            Assert.That(sut, Is.Not.Empty);
            Assert.That(sut, Has.Count.EqualTo(1));
            Assert.That(sut.First().Problems, Is.EquivalentTo(expectedMessages));
        });
    }

    [Test]
    public void When_Cleaning_Error_Collection_Then_Count_Equals_Zero() {
        // arrange
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };

        // act
        ICollection<Error> sut = new ErrorCollection(initial);
        var countBefore = sut.Count;
        sut.Clear();
        var countAfter = sut.Count;

        // assert
        Assert.Multiple(() => {
            Assert.That(sut, Is.Empty);
            Assert.That(countBefore, Is.EqualTo(1));
            Assert.That(countAfter, Is.EqualTo(0));
        });
    }

    [Test]
    public void When_Checking_If_Contains_Element_When_Element_Exists_With_Same_Code_Then_Return_True() {
        // arrange
        var error = new Error("Error Code", ["Message 1", "Message 2"]);
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };
        ICollection<Error> sut = new ErrorCollection(initial);

        // act
        var actual = sut.Contains(error);

        // assert
        Assert.That(actual, Is.True);
    }

    [Test]
    public void When_Checking_If_Contains_Element_When_Element_Not_Exists_Then_Return_False() {
        // arrange
        var error = new Error("Error Code 2", ["Message 1", "Message 2"]);
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 2"] }
        };
        ICollection<Error> sut = new ErrorCollection(initial);

        // act
        var actual = sut.Contains(error);

        // assert
        Assert.That(actual, Is.False);
    }

    [Test]
    public void When_Checking_If_Contains_Element_When_Element_Exists_With_Same_Code_But_Different_Problems_Then_Return_True() {
        // arrange
        var error = new Error("Error Code", ["Message 1", "Message 2"]);
        var initial = new Dictionary<string, string[]> {
            { "Error Code", ["Message 1", "Message 3"] }
        };
        ICollection<Error> sut = new ErrorCollection(initial);

        // act
        var actual = sut.Contains(error);

        // assert
        Assert.That(actual, Is.True);
    }
}
