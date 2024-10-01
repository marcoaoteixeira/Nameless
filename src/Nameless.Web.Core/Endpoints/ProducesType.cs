namespace Nameless.Web.Endpoints;

public enum ProducesType {
    /// <summary>
    /// Default (e.g HTTP status code 200 OK)
    /// </summary>
    Default,

    /// <summary>
    /// Problem (e.g HTTP status code 500 Internal Server Error)
    /// </summary>
    Problems,

    /// <summary>
    /// Validation problem (e.g HTTP status code 400 Bad Request)
    /// </summary>
    ValidationProblems
}