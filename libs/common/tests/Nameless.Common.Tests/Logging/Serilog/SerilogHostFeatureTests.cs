using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Testing.Tools.Mockers.Hosting;
using Serilog.Configuration;
using Serilog.Events;

namespace Nameless.Logging.Serilog;

[UnitTest]
public class SerilogHostFeatureTests {
    [Fact]
    public void Register_WithOverwrittenSinks_ResolvesLoggerAndAppliesCustomConfiguration() {
        // arrange
        var settings = 0;
        var enrichment = 0;
        var sink = 0;
        var minimumLevel = 0;

        var featureContext = new HostFeatureContext([], NullLogger.Instance);
        var hostApplicationBuilder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                                       .Build();

        hostApplicationBuilder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        var sut = new SerilogHostFeature();

        sut.Configure(opts => opts.WithSettingsConfiguration((_, _) => settings++, overwrite: true)
                                  .WithEnrichmentConfiguration((_, _) => enrichment++, overwrite: true)
                                  .WithSinkConfiguration((_, _) => sink++, overwrite: true)
                                  .WithMinimumLevelConfiguration((_, config) => {
                                      minimumLevel++;
                                      config.Is(LogEventLevel.Debug);
                                  }, overwrite: true));

        sut.Register(hostApplicationBuilder, featureContext);

        using var provider = hostApplicationBuilder.Services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<SerilogHostFeatureTests>>();
        logger.LogInformation("hello");

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, settings),
            () => Assert.Equal(1, enrichment),
            () => Assert.Equal(1, sink),
            () => Assert.Equal(1, minimumLevel)
        );
    }

    [Fact]
    public void Register_WithDefaults_ResolvesLogger() {
        // arrange
        var featureContext = new HostFeatureContext([], NullLogger.Instance);
        var hostApplicationBuilder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                                       .Build();

        hostApplicationBuilder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        var sut = new SerilogHostFeature();

        sut.Register(hostApplicationBuilder, featureContext);

        using var provider = hostApplicationBuilder.Services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetRequiredService<ILogger<SerilogHostFeatureTests>>());
    }

    [Fact]
    public void SerilogFeatureOptions_WithMethods_SetDelegatesAndOverwriteFlags() {
        // arrange
        var sut = new SerilogFeatureOptions();
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
            () => Assert.True(sut is {
                OverwriteSettingsConfiguration: true,
                OverwriteEnrichmentConfiguration: true,
                OverwriteSinkConfiguration: true,
                OverwriteMinimumLevelConfiguration: true
            })
        );
    }
}