using Nameless.Testing.Tools.Attributes;

namespace Nameless.Common.Testing.Tools.Attributes;

[UnitTest]
public class RunsOnDevMachineAttributeTests
{
    private const string CATEGORY_NAME = "RunsOnDevMachine";

    [Fact]
    public void WhenGetTraits_MustReturnCorrectCategoryTrait()
    {
        // arrange
        var sut = new RunsOnDevMachineAttribute();

        // act
        var trait = sut.GetTraits().SingleOrDefault(trait => trait.Key == "Category");

        // assert
        Assert.Equal(CATEGORY_NAME, trait.Value);
    }

    [Fact]
    public void WhenGetTraits_ThenReturnsExpectedTraits()
    {
        // arrange
        const string Expected = $"Category={CATEGORY_NAME};Author=";
        var sut = new RunsOnDevMachineAttribute();

        // act
        var traits = string.Join(";", sut.GetTraits().Select(trait => $"{trait.Key}={trait.Value}"));

        // assert
        Assert.Equal(Expected, traits);
    }

    [Fact]
    public void WhenGetTraits_WhenAllTraitsAreFilled_ThenReturnsExpectedTraitsWithValues()
    {
        // arrange
        const string Expected = $"Category={CATEGORY_NAME};Author=Tester";
        var sut = new RunsOnDevMachineAttribute
        {
            Author = "Tester"
        };

        // act
        var traits = string.Join(";", sut.GetTraits().Select(trait => $"{trait.Key}={trait.Value}"));

        // assert
        Assert.Equal(Expected, traits);
    }
}