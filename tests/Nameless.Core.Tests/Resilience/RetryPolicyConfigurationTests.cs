namespace Nameless.Resilience;

public class RetryPolicyConfigurationTests {
    // --- Default ---

    [Fact]
    public void Default_HasNonNullTag() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.False(string.IsNullOrWhiteSpace(config.Tag));
    }

    [Fact]
    public void Default_HasRetryCountOfThree() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.Equal(3, config.RetryCount);
    }

    [Fact]
    public void Default_HasExponentialBackoff() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.Equal(BackoffType.Exponential, config.BackoffType);
    }

    [Fact]
    public void Default_HasPositiveInitialDelay() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.True(config.InitialDelay > TimeSpan.Zero);
    }

    [Fact]
    public void Default_HasPositiveMaxDelay() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.True(config.MaxDelay > TimeSpan.Zero);
    }

    [Fact]
    public void Default_HasJitterEnabled() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.True(config.UseJitter);
    }

    [Fact]
    public void Default_HasOnRetryCallback() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert
        Assert.NotNull(config.OnRetry);
    }

    [Fact]
    public void Default_OnRetry_IsInvocable() {
        // arrange
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // act & assert — invoking the no-op lambda should not throw
        var ex = Record.Exception(() => config.OnRetry(null, TimeSpan.Zero, 0, 0));
        Assert.Null(ex);
    }

    [Fact]
    public void Default_RetryOnException_IsNull() {
        // act
        var config = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // assert — default does not set RetryOnException
        Assert.Null(config.RetryOnException);
    }

    // --- With expression ---

    [Fact]
    public void WithModification_ProducesNewInstanceWithChangedProperty() {
        // arrange
        var original = RetryPolicyConfiguration.CreateDefault((_, _, _, _) => { });

        // act
        var modified = original with { RetryCount = 10 };

        // assert
        Assert.Multiple(() => {
            Assert.Equal(10, modified.RetryCount);
            Assert.Equal(3, original.RetryCount); // original unchanged
        });
    }
}
