namespace Nameless.Web.Http.Endpoints.Generator.Models;

public enum ParameterBindingKind {
    None,
    AsParameters,
    FromBody,
    FromForm,
    FromHeader,
    FromQuery,
    FromRoute,
}
