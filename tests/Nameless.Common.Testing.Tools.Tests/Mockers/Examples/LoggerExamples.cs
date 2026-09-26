using Microsoft.Extensions.Logging;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Common.Testing.Tools.Mockers.Examples;

[UnitTest]
public class LoggerExamples
{
    // Using a callback method to retrieve the log message
    [Fact]
    public void Call_Logger_With_Callback()
    {
        const string Message = "It works!";

        var lines = new List<string>();
        var logger = new LoggerMocker<LoggerExamples>()
            .WithLogDebug((_, message) => lines.Add(message))
            .Build();

        logger.LogDebug(Message);

        Assert.Contains(Message, lines);
    }

    // Verify if a message was logged.
    [Fact]
    public void Call_Logger_Then_Verify()
    {
        const string Message = "It works!";

        var loggerMocker = new LoggerMocker<LoggerExamples>();
            //.WithLog(LogLevel.Debug);
        var logger = loggerMocker.Build();

        logger.LogDebug(Message);

        loggerMocker.VerifyDebug(message => message.Contains(Message));
    }

    // Multiple verifies...
    [Fact]
    public void Call_Logger_Then_Multiple_Verifies()
    {
        const string DebugMessage = "It works for Debug!";
        const string InformationMessage = "It works for Information!";
        const string WarningMessage = "It works for Warning!";

        var loggerMocker = new LoggerMocker<LoggerExamples>()
            .WithAnyLogLevel();
        var logger = loggerMocker.Build();

        logger.LogDebug(DebugMessage);
        logger.LogInformation(InformationMessage);
        logger.LogWarning(WarningMessage);

        loggerMocker
            .VerifyDebug(message => message.Contains(DebugMessage))
            .VerifyInformation(message => message.Contains(InformationMessage))
            .VerifyWarning(message => message.Contains(WarningMessage));
    }

    // Verify more than one call
    [Fact]
    public void Call_Logger_Then_Verify_More_Than_One_Call()
    {
        const string DebugMessage = "It works for Debug!";
        
        var loggerMocker = new LoggerMocker<LoggerExamples>()
            .WithAnyLogLevel();
        var logger = loggerMocker.Build();

        logger.LogDebug(DebugMessage);
        logger.LogDebug(DebugMessage);

        loggerMocker
            .VerifyDebug(message => message.Contains(DebugMessage), times: 2);
    }

    // Verify fails
    [Fact]
    public void Call_Logger_Then_Verify_Fails()
    {
        const string DebugMessage = "It works for Debug!";
        const string FailureMessage = "Whoops...";

        var loggerMocker = new LoggerMocker<LoggerExamples>()
            .WithAnyLogLevel();
        var logger = loggerMocker.Build();

        logger.LogDebug(DebugMessage);
        logger.LogDebug(DebugMessage);

        var exception = Record.Exception(() =>
            loggerMocker.VerifyDebug(message => message.Contains(FailureMessage))
        );

        Assert.IsType<LoggerMockVerificationException>(exception);
    }
}
