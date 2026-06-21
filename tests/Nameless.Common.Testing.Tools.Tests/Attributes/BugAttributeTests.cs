using Nameless.Testing.Tools.Attributes;

namespace Nameless.Common.Testing.Tools.Attributes;

[UnitTest]
public class BugAttributeTests
{
    private const string CATEGORY_NAME = "Bug";

    [Fact]
    public void WhenGetTraits_MustReturnCorrectCategoryTrait()
    {
        // arrange
        var sut = new BugAttribute();

        // act
        var trait = sut.GetTraits().SingleOrDefault(trait => trait.Key == "Category");

        // assert
        Assert.Equal(CATEGORY_NAME, trait.Value);
    }

    [Fact]
    public void WhenGetTraits_ThenReturnsExpectedTraits()
    {
        // arrange
        const string Expected = $"Category={CATEGORY_NAME};Identifier=;Author=";
        var sut = new BugAttribute();

        // act
        var traits = string.Join(";", sut.GetTraits().Select(trait => $"{trait.Key}={trait.Value}"));

        // assert
        Assert.Equal(Expected, traits);
    }

    [Fact]
    public void WhenGetTraits_WhenAllTraitsAreFilled_ThenReturnsExpectedTraitsWithValues()
    {
        // arrange
        const string Expected = $"Category={CATEGORY_NAME};Issue=TICKET-101;Author=Tester";
        var sut = new BugAttribute("TICKET-101")
        {
            Author = "Tester"
        };

        // act
        var traits = string.Join(";", sut.GetTraits().Select(trait => $"{trait.Key}={trait.Value}"));

        // assert
        Assert.Equal(Expected, traits);
    }
}