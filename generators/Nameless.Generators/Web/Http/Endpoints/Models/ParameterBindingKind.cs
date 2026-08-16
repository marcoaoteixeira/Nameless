namespace Nameless.Generators.Web.Http.Endpoints.Models;

public enum ParameterBindingKind {
    None,
    AsParameters,
    FromBody,
    FromForm,
    FromHeader,
    FromQuery,
    FromRoute,
}
