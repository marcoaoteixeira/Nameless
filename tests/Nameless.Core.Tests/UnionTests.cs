namespace Nameless;

public class UnionTests {
    // ─── Parameterless constructor ───────────────────────────────────────────

    [Fact]
    public void DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Union<string, int>());
    }

    // ─── Implicit conversion from TValue0 ─────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromValue0_IsValue0True() {
        // act
        Union<string, int> sw = "hello";

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsValue0);
            Assert.False(sw.IsValue1);
            Assert.Equal(0, sw.Index);
        });
    }

    [Fact]
    public void ImplicitConversion_FromValue0_ValueReturnsValue0() {
        // act
        Union<string, int> sw = "hello";

        // assert
        Assert.Equal("hello", sw.Value);
    }

    // ─── Implicit conversion from TValue1 ─────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromValue1_IsValue1True() {
        // act
        Union<string, int> sw = 42;

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsValue1);
            Assert.False(sw.IsValue0);
            Assert.Equal(1, sw.Index);
        });
    }

    [Fact]
    public void ImplicitConversion_FromValue1_ValueReturnsValue1() {
        // act
        Union<string, int> sw = 42;

        // assert
        Assert.Equal(42, sw.Value);
    }

    // ─── AsValue0 / AsValue1 exceptions ─────────────────────────────────────────

    [Fact]
    public void AsValue0_WhenIsValue1_ThrowsInvalidOperationException() {
        // arrange
        Union<string, int> sw = 42;

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsValue0);
    }

    [Fact]
    public void AsValue1_WhenIsValue0_ThrowsInvalidOperationException() {
        // arrange
        Union<string, int> sw = "hello";

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsValue1);
    }

    // ─── Match(Action, Action) ───────────────────────────────────────────────

    [Fact]
    public void Match_Action_WhenValue0_CallsOnValue0() {
        // arrange
        Union<string, int> sw = "hello";
        var value0Called = false;
        var value1Called = false;

        // act
        sw.Match(
            _ => { value0Called = true; },
            _ => { value1Called = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.True(value0Called);
            Assert.False(value1Called);
        });
    }

    [Fact]
    public void Match_Action_WhenValue1_CallsOnValue1() {
        // arrange
        Union<string, int> sw = 42;
        var value0Called = false;
        var value1Called = false;

        // act
        sw.Match(
            _ => { value0Called = true; },
            _ => { value1Called = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.False(value0Called);
            Assert.True(value1Called);
        });
    }

    // ─── Match(Func, Func) ───────────────────────────────────────────────────

    [Fact]
    public void Match_Func_WhenValue0_ReturnsOnValue0Result() {
        // arrange
        Union<string, int> sw = "hello";

        // act
        var result = sw.Match(
            onValue0: s => $"string:{s}",
            onValue1: n => $"int:{n}"
        );

        // assert
        Assert.Equal("string:hello", result);
    }

    [Fact]
    public void Match_Func_WhenValue1_ReturnsOnValue1Result() {
        // arrange
        Union<string, int> sw = 42;

        // act
        var result = sw.Match(
            onValue0: s => $"string:{s}",
            onValue1: n => $"int:{n}"
        );

        // assert
        Assert.Equal("int:42", result);
    }

    // ─── Union<T0,T1,T2> ───────────────────────────────────────────────────────

    [Fact]
    public void Union3_DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Union<string, int, bool>());
    }

    [Fact]
    public void Union3_FromValue0_IsValue0True() {
        // act
        Union<string, int, bool> sw = "hello";

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsValue0);
            Assert.False(sw.IsValue1);
            Assert.False(sw.IsValue2);
            Assert.Equal(0, sw.Index);
            Assert.Equal("hello", sw.Value);
        });
    }

    [Fact]
    public void Union3_FromValue1_IsValue1True() {
        // act
        Union<string, int, bool> sw = 42;

        // assert
        Assert.Multiple(() => {
            Assert.False(sw.IsValue0);
            Assert.True(sw.IsValue1);
            Assert.False(sw.IsValue2);
            Assert.Equal(1, sw.Index);
            Assert.Equal(42, sw.Value);
        });
    }

    [Fact]
    public void Union3_FromValue2_IsValue2True() {
        // act
        Union<string, int, bool> sw = true;

        // assert
        Assert.Multiple(() => {
            Assert.False(sw.IsValue0);
            Assert.False(sw.IsValue1);
            Assert.True(sw.IsValue2);
            Assert.Equal(2, sw.Index);
            Assert.Equal(true, sw.Value);
        });
    }

    [Fact]
    public void Union3_AsValue0_WhenIsValue1_ThrowsInvalidOperationException() {
        Union<string, int, bool> sw = 42;
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsValue0);
    }

    [Fact]
    public void Union3_AsValue1_WhenIsValue0_ThrowsInvalidOperationException() {
        Union<string, int, bool> sw = "hello";
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsValue1);
    }

    [Fact]
    public void Union3_AsValue2_WhenIsValue0_ThrowsInvalidOperationException() {
        Union<string, int, bool> sw = "hello";
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsValue2);
    }

    [Fact]
    public void Union3_Match_Action_WhenValue0_CallsOnValue0() {
        Union<string, int, bool> sw = "hello";
        var called = string.Empty;

        sw.Match(
            s => { called = $"value0:{s}"; },
            _ => { called = "value1"; },
            _ => { called = "value2"; }
        );

        Assert.Equal("value0:hello", called);
    }

    [Fact]
    public void Union3_Match_Action_WhenValue1_CallsOnValue1() {
        Union<string, int, bool> sw = 42;
        var called = string.Empty;

        sw.Match(
            _ => { called = "value0"; },
            n => { called = $"value1:{n}"; },
            _ => { called = "value2"; }
        );

        Assert.Equal("value1:42", called);
    }

    [Fact]
    public void Union3_Match_Action_WhenValue2_CallsOnValue2() {
        Union<string, int, bool> sw = true;
        var called = string.Empty;

        sw.Match(
            _ => { called = "value0"; },
            _ => { called = "value1"; },
            b => { called = $"value2:{b}"; }
        );

        Assert.Equal("value2:True", called);
    }

    [Fact]
    public void Union3_Match_Func_WhenValue0_ReturnsValue0Result() {
        Union<string, int, bool> sw = "hello";

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("string:hello", result);
    }

    [Fact]
    public void Union3_Match_Func_WhenValue1_ReturnsValue1Result() {
        Union<string, int, bool> sw = 42;

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("int:42", result);
    }

    [Fact]
    public void Union3_Match_Func_WhenValue2_ReturnsValue2Result() {
        Union<string, int, bool> sw = true;

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("bool:True", result);
    }
}
