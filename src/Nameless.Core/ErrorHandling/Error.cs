namespace Nameless.ErrorHandling;

public sealed record Error {
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// Gets the error problems.
    /// </summary>
    public string[] Problems { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="problems">The error problems.</param>
    public Error(string code, string[] problems) {
        Code = Prevent.Argument.Null(code);
        Problems = Prevent.Argument.Null(problems);
    }
}