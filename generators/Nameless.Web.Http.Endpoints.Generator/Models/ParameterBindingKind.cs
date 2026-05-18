namespace Nameless.Web.Http.Endpoints.Generator.Models;

internal enum ParameterBindingKind {
    None,
    FromBody,
    FromRoute,
    FromQuery,
    FromHeader,
    AsParameters,
    FromForm
}
