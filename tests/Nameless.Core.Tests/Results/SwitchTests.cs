using Nameless.Results;

namespace Nameless;

public class SwitchTests {
    // ─── Parameterless constructor ───────────────────────────────────────────

    [Fact]
    public void DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Switch<string, int>());
    }

    // ─── Implicit conversion from TArg0 ─────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromArg0_IsArg0True() {
        // act
        Switch<string, int> sw = "hello";

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsArg0);
            Assert.False(sw.IsArg1);
            Assert.Equal(0, sw.Index);
        });
    }

    [Fact]
    public void ImplicitConversion_FromArg0_ValueReturnsArg0() {
        // act
        Switch<string, int> sw = "hello";

        // assert
        Assert.Equal("hello", sw.Value);
    }

    // ─── Implicit conversion from TArg1 ─────────────────────────────────────

    [Fact]
    public void ImplicitConversion_FromArg1_IsArg1True() {
        // act
        Switch<string, int> sw = 42;

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsArg1);
            Assert.False(sw.IsArg0);
            Assert.Equal(1, sw.Index);
        });
    }

    [Fact]
    public void ImplicitConversion_FromArg1_ValueReturnsArg1() {
        // act
        Switch<string, int> sw = 42;

        // assert
        Assert.Equal(42, sw.Value);
    }

    // ─── AsArg0 / AsArg1 exceptions ─────────────────────────────────────────

    [Fact]
    public void AsArg0_WhenIsArg1_ThrowsInvalidOperationException() {
        // arrange
        Switch<string, int> sw = 42;

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsArg0);
    }

    [Fact]
    public void AsArg1_WhenIsArg0_ThrowsInvalidOperationException() {
        // arrange
        Switch<string, int> sw = "hello";

        // act & assert
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsArg1);
    }

    // ─── Match(Action, Action) ───────────────────────────────────────────────

    [Fact]
    public void Match_Action_WhenArg0_CallsOnArg0() {
        // arrange
        Switch<string, int> sw = "hello";
        var arg0Called = false;
        var arg1Called = false;

        // act
        sw.Match(
            _ => { arg0Called = true; },
            _ => { arg1Called = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.True(arg0Called);
            Assert.False(arg1Called);
        });
    }

    [Fact]
    public void Match_Action_WhenArg1_CallsOnArg1() {
        // arrange
        Switch<string, int> sw = 42;
        var arg0Called = false;
        var arg1Called = false;

        // act
        sw.Match(
            _ => { arg0Called = true; },
            _ => { arg1Called = true; }
        );

        // assert
        Assert.Multiple(() => {
            Assert.False(arg0Called);
            Assert.True(arg1Called);
        });
    }

    // ─── Match(Func, Func) ───────────────────────────────────────────────────

    [Fact]
    public void Match_Func_WhenArg0_ReturnsOnArg0Result() {
        // arrange
        Switch<string, int> sw = "hello";

        // act
        var result = sw.Match(
            onArg0: s => $"string:{s}",
            onArg1: n => $"int:{n}"
        );

        // assert
        Assert.Equal("string:hello", result);
    }

    [Fact]
    public void Match_Func_WhenArg1_ReturnsOnArg1Result() {
        // arrange
        Switch<string, int> sw = 42;

        // act
        var result = sw.Match(
            onArg0: s => $"string:{s}",
            onArg1: n => $"int:{n}"
        );

        // assert
        Assert.Equal("int:42", result);
    }

    // ─── Switch<T0,T1,T2> ───────────────────────────────────────────────────────

    [Fact]
    public void Switch3_DefaultConstructor_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => new Switch<string, int, bool>());
    }

    [Fact]
    public void Switch3_FromArg0_IsArg0True() {
        // act
        Switch<string, int, bool> sw = "hello";

        // assert
        Assert.Multiple(() => {
            Assert.True(sw.IsArg0);
            Assert.False(sw.IsArg1);
            Assert.False(sw.IsArg2);
            Assert.Equal(0, sw.Index);
            Assert.Equal("hello", sw.Value);
        });
    }

    [Fact]
    public void Switch3_FromArg1_IsArg1True() {
        // act
        Switch<string, int, bool> sw = 42;

        // assert
        Assert.Multiple(() => {
            Assert.False(sw.IsArg0);
            Assert.True(sw.IsArg1);
            Assert.False(sw.IsArg2);
            Assert.Equal(1, sw.Index);
            Assert.Equal(42, sw.Value);
        });
    }

    [Fact]
    public void Switch3_FromArg2_IsArg2True() {
        // act
        Switch<string, int, bool> sw = true;

        // assert
        Assert.Multiple(() => {
            Assert.False(sw.IsArg0);
            Assert.False(sw.IsArg1);
            Assert.True(sw.IsArg2);
            Assert.Equal(2, sw.Index);
            Assert.Equal(true, sw.Value);
        });
    }

    [Fact]
    public void Switch3_AsArg0_WhenIsArg1_ThrowsInvalidOperationException() {
        Switch<string, int, bool> sw = 42;
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsArg0);
    }

    [Fact]
    public void Switch3_AsArg1_WhenIsArg0_ThrowsInvalidOperationException() {
        Switch<string, int, bool> sw = "hello";
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsArg1);
    }

    [Fact]
    public void Switch3_AsArg2_WhenIsArg0_ThrowsInvalidOperationException() {
        Switch<string, int, bool> sw = "hello";
        Assert.Throws<InvalidOperationException>(() => _ = sw.AsArg2);
    }

    [Fact]
    public void Switch3_Match_Action_WhenArg0_CallsOnArg0() {
        Switch<string, int, bool> sw = "hello";
        var called = string.Empty;

        sw.Match(
            s => { called = $"arg0:{s}"; },
            _ => { called = "arg1"; },
            _ => { called = "arg2"; }
        );

        Assert.Equal("arg0:hello", called);
    }

    [Fact]
    public void Switch3_Match_Action_WhenArg1_CallsOnArg1() {
        Switch<string, int, bool> sw = 42;
        var called = string.Empty;

        sw.Match(
            _ => { called = "arg0"; },
            n => { called = $"arg1:{n}"; },
            _ => { called = "arg2"; }
        );

        Assert.Equal("arg1:42", called);
    }

    [Fact]
    public void Switch3_Match_Action_WhenArg2_CallsOnArg2() {
        Switch<string, int, bool> sw = true;
        var called = string.Empty;

        sw.Match(
            _ => { called = "arg0"; },
            _ => { called = "arg1"; },
            b => { called = $"arg2:{b}"; }
        );

        Assert.Equal("arg2:True", called);
    }

    [Fact]
    public void Switch3_Match_Func_WhenArg0_ReturnsArg0Result() {
        Switch<string, int, bool> sw = "hello";

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("string:hello", result);
    }

    [Fact]
    public void Switch3_Match_Func_WhenArg1_ReturnsArg1Result() {
        Switch<string, int, bool> sw = 42;

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("int:42", result);
    }

    [Fact]
    public void Switch3_Match_Func_WhenArg2_ReturnsArg2Result() {
        Switch<string, int, bool> sw = true;

        var result = sw.Match(
            s => $"string:{s}",
            n => $"int:{n}",
            b => $"bool:{b}"
        );

        Assert.Equal("bool:True", result);
    }
}
