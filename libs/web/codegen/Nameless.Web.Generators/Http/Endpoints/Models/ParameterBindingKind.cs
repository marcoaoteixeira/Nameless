namespace Nameless.Web.Generators.Http.Endpoints.Models;

public enum ParameterBindingKind {
    None,
    AsParameters,
    FromBody,
    FromForm,
    FromHeader,
    FromQuery,
    FromRoute,
}
