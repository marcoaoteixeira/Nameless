namespace Nameless.Web.Generators.Models;

public enum ParameterBindingKind {
    None,
    AsParameters,
    FromBody,
    FromForm,
    FromHeader,
    FromQuery,
    FromRoute,
}
