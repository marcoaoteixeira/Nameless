using Nameless.Null;

namespace Nameless;

public class NullObjectTests {
    // ─── NullDisposable ──────────────────────────────────────────────────────

    [Fact]
    public void NullDisposable_Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullDisposable.Instance, NullDisposable.Instance);
    }

    [Fact]
    public void NullDisposable_Dispose_DoesNotThrow() {
        // arrange
        var disposable = NullDisposable.Instance;

        // act
        var ex = Record.Exception(disposable.Dispose);

        // assert
        Assert.Null(ex);
    }

    // ─── NullProgress<T> ─────────────────────────────────────────────────────

    [Fact]
    public void NullProgressGeneric_Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullProgress<string>.Instance, NullProgress<string>.Instance);
    }

    [Fact]
    public void NullProgressGeneric_Report_DoesNotThrow() {
        // arrange
        var progress = NullProgress<string>.Instance;

        // act
        var ex = Record.Exception(() => progress.Report("value"));

        // assert
        Assert.Null(ex);
    }

    // ─── NullProgress (non-generic) ──────────────────────────────────────────

    [Fact]
    public void NullProgress_Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullProgress.Instance, NullProgress.Instance);
    }

    [Fact]
    public void NullProgress_Report_DoesNotThrow() {
        // arrange
        var progress = NullProgress.Instance;

        // act
        var ex = Record.Exception(() => progress.Report(0));

        // assert
        Assert.Null(ex);
    }

    // ─── NullServiceProvider ─────────────────────────────────────────────────

    [Fact]
    public void NullServiceProvider_Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullServiceProvider.Instance, NullServiceProvider.Instance);
    }

    [Fact]
    public void NullServiceProvider_GetService_ReturnsNull() {
        // act
        var result = NullServiceProvider.Instance.GetService(typeof(string));

        // assert
        Assert.Null(result);
    }

    // ─── NullDictionary<K, V> ────────────────────────────────────────────────

    [Fact]
    public void NullDictionary_Instance_ReturnsSameSingleton() {
        // act & assert
        Assert.Same(NullDictionary<string, int>.Instance, NullDictionary<string, int>.Instance);
    }

    [Fact]
    public void NullDictionary_Count_IsZero() {
        // act & assert
        Assert.Empty(NullDictionary<string, int>.Instance);
    }

    [Fact]
    public void NullDictionary_IsReadOnly_IsTrue() {
        // act & assert
        Assert.True(NullDictionary<string, int>.Instance.IsReadOnly);
    }

    [Fact]
    public void NullDictionary_ContainsKey_AlwaysReturnsFalse() {
        // act & assert
        Assert.False(NullDictionary<string, int>.Instance.ContainsKey("anything"));
    }

    [Fact]
    public void NullDictionary_TryGetValue_AlwaysReturnsFalse() {
        // act
        var found = NullDictionary<string, int>.Instance.TryGetValue("key", out var value);

        // assert
        Assert.Multiple(() => {
            Assert.False(found);
            Assert.Equal(default, value);
        });
    }

    [Fact]
    public void NullDictionary_Add_DoesNotThrow() {
        // arrange
        var dict = NullDictionary<string, int>.Instance;

        // act
        var ex = Record.Exception(() => dict.Add("key", 1));

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public void NullDictionary_Remove_DoesNotThrow() {
        // arrange
        var dict = NullDictionary<string, int>.Instance;

        // act
        var ex = Record.Exception(() => dict.Remove("key"));

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public void NullDictionary_Clear_DoesNotThrow() {
        // arrange
        var dict = NullDictionary<string, int>.Instance;

        // act
        var ex = Record.Exception(dict.Clear);

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public void NullDictionary_IndexerGet_ReturnsDefault() {
        // act
        var result = NullDictionary<string, int>.Instance["key"];

        // assert
        Assert.Equal(default, result);
    }
}
