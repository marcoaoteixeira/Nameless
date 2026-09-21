using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Nameless.Web.RateLimiter;

[UnitTest]
public class RateLimiterOptionsExtensionsTests {
    // --- Argument validation -------------------------------------------------

    [Fact]
    public void AddPolicy_NullOptions_ThrowsArgumentNullException() {
        RateLimiterOptions? options = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            options!.AddPolicy(typeof(string), typeof(TestPartitionPolicy), "name"));

        Assert.Equal("options", ex.ParamName);
    }

    [Fact]
    public void AddPolicy_NullPartitionKeyType_ThrowsArgumentNullException() {
        var options = new RateLimiterOptions();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            options.AddPolicy(null!, typeof(TestPartitionPolicy), "name"));

        Assert.Equal("partitionKeyType", ex.ParamName);
    }

    [Fact]
    public void AddPolicy_NullRateLimiterPolicyType_ThrowsArgumentNullException() {
        var options = new RateLimiterOptions();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            options.AddPolicy(typeof(string), null!, "name"));

        Assert.Equal("rateLimiterPolicyType", ex.ParamName);
    }

    [Fact]
    public void AddPolicy_NullName_ThrowsArgumentNullException() {
        var options = new RateLimiterOptions();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            options.AddPolicy(typeof(string), typeof(TestPartitionPolicy), null!));

        Assert.Equal("name", ex.ParamName);
    }

    // --- Type-compatibility guard ---------------------------------------------

    [Fact]
    public void AddPolicy_PolicyTypeImplementsDifferentPartitionKey_ThrowsArgumentException() {
        var options = new RateLimiterOptions();

        // WrongPartitionKeyPolicy implements IRateLimiterPolicy<int>, not IRateLimiterPolicy<string>.
        var ex = Assert.Throws<ArgumentException>(() =>
            options.AddPolicy(typeof(string), typeof(WrongPartitionKeyPolicy), "mismatch"));

        Assert.Equal("rateLimiterPolicyType", ex.ParamName);
    }

    [Fact]
    public void AddPolicy_PolicyTypeDoesNotImplementInterfaceAtAll_ThrowsArgumentException() {
        var options = new RateLimiterOptions();

        var ex = Assert.Throws<ArgumentException>(() =>
            options.AddPolicy(typeof(string), typeof(object), "no-interface"));

        Assert.Equal("rateLimiterPolicyType", ex.ParamName);
    }

    // --- Happy path + proof the real framework method actually ran ------------

    [Fact]
    public void AddPolicy_ValidTypes_DoesNotThrow() {
        var options = new RateLimiterOptions();

        var exception = Record.Exception(() =>
            options.AddPolicy(typeof(string), typeof(TestPartitionPolicy), "valid-policy"));

        Assert.Null(exception);
    }

    [Fact]
    public void AddPolicy_DuplicateName_ThrowsArgumentException() {
        // RateLimiterOptions.PolicyMap/UnactivatedPolicyMap are internal, so we can't assert
        // on registration state directly. But the genuine AddPolicy<TPartitionKey, TPolicy>
        // throws on a duplicate name, so getting that exact exception back proves the
        // reflection call actually reached the real framework method instead of silently
        // doing nothing.
        var options = new RateLimiterOptions();
        options.AddPolicy(typeof(string), typeof(TestPartitionPolicy), "duplicate");

        var ex = Assert.Throws<ArgumentException>(() =>
            options.AddPolicy(typeof(string), typeof(TestPartitionPolicy), "duplicate"));

        Assert.Contains("duplicate", ex.Message);
    }

    // --- Test fixtures ----------------------------------------------------------

    private sealed class TestPartitionPolicy : IRateLimiterPolicy<string> {
        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => null;

        public RateLimitPartition<string> GetPartition(HttpContext httpContext) {
            return RateLimitPartition.GetNoLimiter("test");
        }
    }

    private sealed class WrongPartitionKeyPolicy : IRateLimiterPolicy<int> {
        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => null;

        public RateLimitPartition<int> GetPartition(HttpContext httpContext) {
            return RateLimitPartition.GetNoLimiter(0);
        }
    }
}

