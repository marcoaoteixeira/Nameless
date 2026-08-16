namespace Nameless.Web.Http.Endpoints;

/// <summary>
///     HTTP verbs
/// </summary>
public enum HttpVerbs {
    /// <summary>
    ///     HTTP GET method.
    /// </summary>
    Get,

    /// <summary>
    ///     HTTP DELETE method.
    /// </summary>
    Delete,
    
    /// <summary>
    ///     HTTP HEAD method.
    /// </summary>
    Head,
    
    /// <summary>
    ///     HTTP OPTIONS method.
    /// </summary>
    Options,
    
    /// <summary>
    ///     HTTP PATCH method.
    /// </summary>
    Patch,
    
    /// <summary>
    ///     HTTP POST method.
    /// </summary>
    Post,
    
    /// <summary>
    ///     HTTP PUT method.
    /// </summary>
    Put,
    
    /// <summary>
    ///     HTTP QUERY method.
    /// </summary>
    Query
}
