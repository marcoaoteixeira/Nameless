namespace Nameless.Generators.Web.Http.Endpoints;

internal static class AutoEndpointsConstants {
    internal static class Project {
        internal const string Name = "Nameless Auto Endpoints Generator";

        internal const string RegistrationClassName = "AutoEndpointsExtensions";
        internal const string RegisterMethodName = "RegisterAutoEndpoints";
        internal const string MapMethodName = "MapAutoEndpoints";

        internal static class Namespaces {
            internal const string Root = "Nameless.Web.Http.Endpoints";

            internal static class Attributes {
                internal const string Versioning = $"{Root}.Attributes.Versioning";
            }
        }

        internal static class Classes {
            internal static class Names {
                internal const string EndpointAttribute = "EndpointAttribute";
                internal const string EndpointGroupAttribute = "EndpointGroupAttribute";
            }

            internal static class FullNames {
                internal const string EndpointAttribute = $"{Namespaces.Root}.EndpointAttribute";
                internal const string EndpointGroupAttribute = $"{Namespaces.Root}.EndpointGroupAttribute";
            }
        }
    }
    
    internal static class EndpointClass {
        internal const string HandlerMethodName = "HandleAsync";
    }

    internal static class EndpointGroupClass {
        internal const string ReservedName = "_SyntheticGroup_";
    }

    internal static class ContentTypes {
        internal const string Json = "application/json";
        internal const string JsonProblem = "application/problem+json";
    }

    // Fully Qualified Names
    internal static class FQN {
        internal const string CancellationToken = "global::System.Threading.CancellationToken";

        internal const string AsParametersAttribute = "global::Microsoft.AspNetCore.Http.AsParametersAttribute";
        internal const string FromBodyAttribute = "global::Microsoft.AspNetCore.Mvc.FromBodyAttribute";
        internal const string FromFormAttribute = "global::Microsoft.AspNetCore.Mvc.FromFormAttribute";
        internal const string FromHeaderAttribute = "global::Microsoft.AspNetCore.Mvc.FromHeaderAttribute";
        internal const string FromQueryAttribute = "global::Microsoft.AspNetCore.Mvc.FromQueryAttribute";
        internal const string FromRouteAttribute = "global::Microsoft.AspNetCore.Mvc.FromRouteAttribute";
    }
}
