﻿using System.Collections;

namespace Nameless;

public class GuardExtensionTests {
    [Test]
    public void Null_Should_Throw_Exception_If_Parameter_Is_Null() {
        // arrange
        object paramValue = null;
        var paramName = "NullParameter";
        var message = $"Argument cannot be null. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(
                () => PreventExtension.Null(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void Null_Should_Not_Throw_Exception_If_Parameter_Is_Not_Null() {
        // arrange
        object paramValue = "123";
        var paramName = "NotNullParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.Null(Prevent.Argument, paramValue, paramName)
        );
    }

    [Test]
    public void NullOrEmpty_String_Should_Throw_Exception_If_Parameter_Is_Null() {
        // arrange
        string paramValue = null;
        var paramName = "NullOrEmptyParameter";
        var message = $"Argument cannot be null. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(
                () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrEmpty_String_Should_Throw_Exception_If_Parameter_Is_Empty() {
        // arrange
        var paramValue = "";
        var paramName = "NullOrEmptyParameter";
        var message = $"Argument cannot be empty. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(
                () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrEmpty_String_Should_Not_Throw_If_Parameter_Is_Not_Null_Nor_Empty() {
        // arrange
        var paramValue = "123";
        var paramName = "NullOrEmptyParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
        );
    }

    [Test]
    public void NullOrWhiteSpace_String_Should_Throw_Exception_If_Parameter_Is_Null() {
        // arrange
        string paramValue = null;
        var paramName = "NullOrWhiteSpaceParameter";
        var message = $"Argument cannot be null. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(
                () => PreventExtension.NullOrWhiteSpace(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrWhiteSpace_String_Should_Throw_Exception_If_Parameter_Is_Empty() {
        // arrange
        var paramValue = "  ";
        var paramName = "NullOrWhiteSpaceParameter";
        var message = $"Argument cannot be white spaces. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(
                () => PreventExtension.NullOrWhiteSpace(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrWhiteSpace_String_Should_Not_Throw_If_Parameter_Is_Not_Null_Nor_WhiteSpace() {
        // arrange
        var paramValue = "123";
        var paramName = "NullOrWhiteSpaceParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.NullOrWhiteSpace(Prevent.Argument, paramValue, paramName)
        );
    }

    [Test]
    public void NullOrEmpty_Array_Should_Throw_If_Parameter_Is_Null() {
        // arrange
        int[] paramValue = null;
        var paramName = "NullOrEmptyParameter";
        var message = $"Argument cannot be null. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(
                () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrEmpty_Array_Should_Throw_If_Parameter_Is_Empty() {
        // arrange
        var paramValue = Array.Empty<int>();
        var paramName = "NullOrEmptyParameter";
        var message = $"Argument cannot be empty. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(
                () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrEmpty_Enumerable_Should_Throw_If_Parameter_Is_Empty() {
        // arrange
        static IEnumerable paramValue() {
            foreach (var item in Array.Empty<int>()) {
                yield return item;
            }
        };
        var paramName = "NullOrEmptyParameter";
        var message = $"Argument cannot be empty. (Parameter '{paramName}')";

        // act

        // assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(
                () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue(), paramName)
            );
            Assert.That(exception?.Message, Is.EqualTo(message));
        });
    }

    [Test]
    public void NullOrEmpty_Array_Should_Not_Throw_If_Parameter_Is_Not_Null_Nor_Empty() {
        // arrange
        var paramValue = new[] { 1, 2, 3 };
        var paramName = "NullOrEmptyParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue, paramName)
        );
    }

    [Test]
    public void NullOrEmpty_Enumerable_Should_Not_Throw_If_Parameter_Is_Not_Null_Nor_Empty() {
        // arrange
        static IEnumerable paramValue() {
            foreach (var item in new[] { 1, 2, 3 }) {
                yield return item;
            }
        };
        var paramName = "NullOrEmptyParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.NullOrEmpty(Prevent.Argument, paramValue(), paramName)
        );
    }

    [Test]
    public void NoMatchingPattern_Should_Throw_Exception_If_Regexp_Fail_To_Match() {
        // arrange
        var paramValue = "ABC";
        var paramPattern = "[a-z]";
        var paramName = "NoMatchingPatternParameter";

        // act

        // assert
        Assert.Throws<ArgumentException>(
            () => PreventExtension.NoMatchingPattern(Prevent.Argument, paramValue, paramName, paramPattern)
        );
    }

    [Test]
    public void NoMatchingPattern_Should_Not_Throw_Exception_If_Regexp_Succeeded_To_Match() {
        // arrange
        const string paramValue = "abc";
        const string paramPattern = "[a-z]+";
        const string paramName = "NoMatchingPatternParameter";

        // act

        // assert
        Assert.DoesNotThrow(
            () => PreventExtension.NoMatchingPattern(Prevent.Argument, paramValue, paramPattern, paramName)
        );
    }
}