using Nameless.Bootstrap;

namespace Nameless.Microservices.App.Configs;

public static class BootstrapConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureBootstrap() {
            self.Services.RegisterBootstrap(opts => {
                opts
                    .RegisterStep<FirstLoggerStep>()
                    .RegisterStep<SecondLoggerStep>()
                    .RegisterStep<ThirdLoggerStep>();
            });

            return self;
        }
    }
}

public class FirstLoggerStep : StepBase {
    private readonly ILogger<FirstLoggerStep> _logger;

    public FirstLoggerStep(ILogger<FirstLoggerStep> logger) {
        _logger = logger;
    }

    public override Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken) {
        _logger.LogDebug("[{LoggerName}]: Yep, I'm working.", nameof(FirstLoggerStep));

        return Task.CompletedTask;
    }
}

public class SecondLoggerStep : StepBase {
    private readonly ILogger<SecondLoggerStep> _logger;

    public SecondLoggerStep(ILogger<SecondLoggerStep> logger) {
        _logger = logger;
    }

    public override Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken) {
        _logger.LogDebug("[{LoggerName}]: Yep, I'm working.", nameof(SecondLoggerStep));

        return Task.CompletedTask;
    }
}

public class ThirdLoggerStep : StepBase {
    private readonly ILogger<ThirdLoggerStep> _logger;

    public ThirdLoggerStep(ILogger<ThirdLoggerStep> logger) {
        _logger = logger;
    }

    public override Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken) {
        _logger.LogDebug("[{LoggerName}]: Yep, I'm working.", nameof(ThirdLoggerStep));

        return Task.CompletedTask;
    }
}