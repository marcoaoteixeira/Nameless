using Nameless.Fixtures;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Collections.Generic;

[UnitTest]
public class PageTests
{
    [Fact]
    public void WhenCreatingPage_ThenReturnsPage()
    {
        // arrange
        const int Start = 0;
        const int Limit = 10;
        const int Number = 1;
        const int TotalCount = 100;
        const int PageCount = 10;
        const bool HasPrevious = false;
        const bool HasNext = true;

        var cars = CarFaker.Instance.Generate(TotalCount);
        var set = cars.Skip(Start).Take(Limit).ToArray();

        // act
        var sut = new Page<Car>(set, Start, Limit, TotalCount);

        // assert
        Assert.Multiple(() =>
        {
            Assert.Equal(set, sut);
            Assert.Equal(Start, sut.Start);
            Assert.Equal(Limit, sut.Limit);
            Assert.Equal(Number, sut.Number);
            Assert.Equal(TotalCount, sut.TotalCount);
            Assert.Equal(PageCount, sut.PageCount);
            Assert.Equal(HasPrevious, sut.HasPrevious);
            Assert.Equal(HasNext, sut.HasNext);
        });
    }

    [Fact]
    public void WhenEnumerating_ThenShouldLoopThroughCorrectItems()
    {
        // arrange
        const int Start = 0;
        const int Limit = 10;
        const int TotalCount = 10;

        var cars = CarFaker.Instance.Generate(TotalCount);

        // act
        var sut = new Page<Car>(cars, Start, Limit, TotalCount);

        var output = new List<Car>();

        foreach (var car in sut)
        {
            output.Add(car);
        }

        // assert
        Assert.Equal(cars, output);
    }

    [Fact]
    public void WhenIndexingItem_ThenShouldReturnExpectedItem()
    {
        // arrange
        const int Index = 6; // 7th item
        const int Start = 0;
        const int Limit = 10;
        const int TotalCount = 10;

        var cars = CarFaker.Instance.Generate(TotalCount);
        var car = cars[Index];

        // act
        var sut = new Page<Car>(cars, Start, Limit, TotalCount);

        // assert
        Assert.Equal(car, sut[Index]);
    }
}
