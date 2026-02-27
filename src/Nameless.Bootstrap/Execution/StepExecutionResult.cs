namespace Nameless.Bootstrap.Execution;

public class StepExecutionResult {
    public required string StepName { get; set; }
    
    public DateTimeOffset StartTime { get; set; }
    
    public TimeSpan Duration { get; set; }
    
    public Exception? Exception { get; set; }

    public bool Success => Exception is null;
}