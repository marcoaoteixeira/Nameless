using System.Collections;
using System.Collections.ObjectModel;
using Nameless.Fixtures;

namespace Nameless;

public class EnumerableExtensionTests {
    [Test]
    public void Each_Should_Interate_Through_Enumerable() {
        // arrange
        var array = new int[] { 1, 2, 3, 4, 5 };
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new List<int>();

        // act
        EnumerableExtension.Each(array, item => actual.Add(item * item));

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void Each_With_Index_Should_Interate_Through_Enumerable() {
        // arrange
        var array = new int[] { 1, 2, 3, 4, 5 };
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new int[5];

        // act
        EnumerableExtension.Each(array, (item, idx) => actual[idx] = item * item);

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void Each_Should_Interate_Through_Enumerable_Function() {
        // arrange
        static IEnumerable<int> GetEnumerable() {
            for (var i = 1; i < 6; i++) {
                yield return i;
            }
        }
        var array = GetEnumerable();
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new List<int>();

        // act
        EnumerableExtension.Each(array, item => actual.Add(item * item));

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void Each_Should_Interate_Through_Enumerable_Function_Non_Generic() {
        // arrange
        static IEnumerable GetEnumerable() {
            for (var i = 1; i < 6; i++) {
                yield return i;
            }
        }
        var array = GetEnumerable();
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new List<int>();

        // act
        EnumerableExtension.Each(array, item => actual.Add(Convert.ToInt32(item) * Convert.ToInt32(item)));

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void IsNullOrEmpty_Should_Return_True_If_Null() {
        // arrange
        int[]? array = null;

        // act
        var result = EnumerableExtension.IsNullOrEmpty(array);

        // assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullOrEmpty_Should_Return_True_If_Empty() {
        // arrange
        var array = Array.Empty<int>();

        // act
        var result = EnumerableExtension.IsNullOrEmpty(array);

        // assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsNullOrEmpty_Should_Return_False_If_Not_Null_Nor_Empty() {
        // arrange
        var array = new[] { 1 };

        // act
        var result = EnumerableExtension.IsNullOrEmpty(array);

        // assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsNullOrEmpty_Should_Return_True_If_Empty_Enumerable() {
        // arrange
        static IEnumerable GetEnumerable() {
            for (var i = 0; i < 0; i++) {
                yield return i;
            }
        }

        var enumerable = GetEnumerable();

        // act
        var result = EnumerableExtension.IsNullOrEmpty(enumerable);

        // assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Each_Non_Generic_Should_Interate_Through_Enumerable() {
        // arrange
        IEnumerable array = new int[] { 1, 2, 3, 4, 5 };
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new List<int>();

        // act
        EnumerableExtension.Each(array, item => actual.Add(Convert.ToInt32(item) * Convert.ToInt32(item)));

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void Each_Non_Generic_With_Index_Should_Interate_Through_Enumerable() {
        // arrange
        IEnumerable array = new int[] { 1, 2, 3, 4, 5 };
        var expected = new int[] { 1, 4, 9, 16, 25 };
        var actual = new int[5];

        // act
        EnumerableExtension.Each(array, (item, idx) => actual[idx] = Convert.ToInt32(item) * Convert.ToInt32(item));

        // assert
        Assert.That(actual.ToArray(), Is.EquivalentTo(expected));
    }

    [Test]
    public void ToReadOnly_Should_Return_ReadOnlyCollection() {
        // arrange
        var array = new int[] { 1, 2, 3, 4, 5 };

        // act
        var actual = EnumerableExtension.ToReadOnly(array);

        // assert
        Assert.Multiple(() => {
            Assert.That(actual, Is.InstanceOf<ReadOnlyCollection<int>>());
            Assert.That(actual, Is.EquivalentTo(array));
        });
    }

    [Test]
    public void DistinctBy_Should_Filter_Distinct_By_Property_Of_Object() {
        // arrange
        var array = new[] {
            new Student {
                Name = "John",
                Age = 20,
                Birthday = DateTime.Now.AddYears(-20).Date
            },
            new Student {
                Name = "John",
                Age = 20,
                Birthday = DateTime.Now.AddYears(-20).Date
            },
            new Student {
                Name = "Chris",
                Age = 21,
                Birthday = DateTime.Now.AddYears(-21).Date
            },
        };
        var expected = new[] {
            new Student {
                Name = "John",
                Age = 20,
                Birthday = DateTime.Now.AddYears(-20).Date
            },
            new Student {
                Name = "Chris",
                Age = 21,
                Birthday = DateTime.Now.AddYears(-21).Date
            }
        };

        // act
        var actual = EnumerableExtension.DistinctBy(array, student => student.Name);

        // assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }
}