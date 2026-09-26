using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Logging.Serilog;
using Nameless.Testing.Tools.Mockers.Logging;
using Serilog.Configuration;
using Serilog.Events;

namespace Nameless.Logging;

[UnitTest]
public class StopwatchLoggerFormattingTests {
    public sealed class Sample;

    [Fact]
    public void StartStopwatchLogger_WithProxyLogger_UsesUnknownClassName() {
        // arrange
        var mocker = new LoggerMocker<Sample>().WithAnyLogLevel();
        var logger = mocker.Build();

        // act
        using (logger.StartStopwatchLogger(caller: "Action")) { }

        // assert
        mocker.VerifyDebug(message => message.Contains("Unknown", StringComparison.Ordinal) && message.Contains("Action", StringComparison.Ordinal), times: 2);
    }

    [Fact]
    public void StartStopwatchLogger_MessageFormat_PutsDurationBeforeMessage() {
        // arrange
        var mocker = new LoggerMocker<Sample>().WithAnyLogLevel();
        var logger = mocker.Build();

        // act
        using (var stopwatch = logger.StartStopwatchLogger(caller: "Action")) {
            stopwatch.Write("custom-message");
        }

        // assert
        mocker.VerifyDebug(message => message.EndsWith("custom-message", StringComparison.Ordinal));
    }

    [Fact]
    public void StartStopwatchLogger_WithDisabledLevel_DoesNotLog() {
        // arrange
        var mocker = new LoggerMocker<Sample>();
        var logger = mocker.Build();

        // act
        var exception = Record.Exception(() => { using var _ = logger.StartStopwatchLogger(); });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Write_AfterDispose_IsIgnored() {
        // arrange
        var logger = new LoggerMocker<Sample>().WithAnyLogLevel().Build();
        var stopwatch = logger.StartStopwatchLogger();
        stopwatch.Dispose();

        // act
        var exception = Record.Exception(() => {
            stopwatch.Write("ignored");
            stopwatch.Dispose();
        });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void StartStopwatchLogger_WithNonGenericLogger_UsesUnknownOrCategoryName() {
        // arrange
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

        // act
        var exception = Record.Exception(() => { using var _ = logger.StartStopwatchLogger(); });

        // assert
        Assert.Null(exception);
    }
}

[UnitTest]
public class SerilogRegistrationTests {
    [Fact]
    public void RegisterSerilog_WithOverwrittenSinks_ResolvesLoggerAndAppliesCustomConfiguration() {
        // arrange
        var settings = 0;
        var enrichment = 0;
        var sink = 0;
        var minimumLevel = 0;

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        var returned = services.RegisterSerilog(registration => registration
            .WithSettingsConfiguration((_, _) => settings++, overwrite: true)
            .WithEnrichmentConfiguration((_, _) => enrichment++, overwrite: true)
            .WithSinkConfiguration((_, _) => sink++, overwrite: true)
            .WithMinimumLevelConfiguration((_, config) => { minimumLevel++; config.Is(LogEventLevel.Debug); }, overwrite: true));

        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<SerilogRegistrationTests>>();
        logger.LogInformation("hello");

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.Equal(1, settings),
            () => Assert.Equal(1, enrichment),
            () => Assert.Equal(1, sink),
            () => Assert.Equal(1, minimumLevel)
        );
    }

    [Fact]
    public void RegisterSerilog_WithDefaults_ResolvesLogger() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        services.RegisterSerilog(configure: null);
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetRequiredService<ILogger<SerilogRegistrationTests>>());
    }

    [Fact]
    public void SerilogRegistration_WithMethods_SetDelegatesAndOverwriteFlags() {
        // arrange
        var sut = new SerilogRegistration();
        Action<IServiceProvider, LoggerSettingsConfiguration> settings = (_, _) => { };
        Action<IServiceProvider, LoggerEnrichmentConfiguration> enrichment = (_, _) => { };
        Action<IServiceProvider, LoggerSinkConfiguration> sink = (_, _) => { };
        Action<IServiceProvider, LoggerMinimumLevelConfiguration> minimum = (_, _) => { };

        // act
        var returned = sut.WithSettingsConfiguration(settings, true)
                          .WithEnrichmentConfiguration(enrichment, true)
                          .WithSinkConfiguration(sink, true)
                          .WithMinimumLevelConfiguration(minimum, true);

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Same(settings, sut.ConfigureSettings),
            () => Assert.Same(enrichment, sut.ConfigureEnrichment),
            () => Assert.Same(sink, sut.ConfigureSink),
            () => Assert.Same(minimum, sut.ConfigureMinimumLevel),
            () => Assert.True(sut.OverwriteSettingsConfiguration && sut.OverwriteEnrichmentConfiguration
                              && sut.OverwriteSinkConfiguration && sut.OverwriteMinimumLevelConfiguration)
        );
    }
}
