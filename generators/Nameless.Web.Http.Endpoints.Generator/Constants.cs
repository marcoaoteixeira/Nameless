namespace Nameless.Web.Http.Endpoints.Generator;

internal static class Constants {
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
                internal const string EndpointAttribute = "EndpointAttribute<THttpVerb>";
                internal const string EndpointGroupAttribute = "EndpointGroupAttribute";
            }

            internal static class FullNames {
                internal const string EndpointAttributeWithArity = $"{Namespaces.Root}.EndpointAttribute`1";
                internal const string EndpointGroupAttribute = $"{Namespaces.Root}.EndpointGroupAttribute";
            }

            internal static class FullyQualifiedNames {
                internal const string HttpVerbDelete = $"global::{Namespaces.Root}.Delete";
                internal const string HttpVerbHead = $"global::{Namespaces.Root}.Head";
                internal const string HttpVerbOptions = $"global::{Namespaces.Root}.Options";
                internal const string HttpVerbPatch = $"global::{Namespaces.Root}.Patch";
                internal const string HttpVerbPost = $"global::{Namespaces.Root}.Post";
                internal const string HttpVerbPut = $"global::{Namespaces.Root}.Put";
            }
        }
    }
    
    internal static class EndpointClass {
        internal const string HandlerMethodName = "HandleAsync";
    }

    internal static class EndpointGroupClass {
        internal const string ReservedName = "_SyntheticEndpointGroup_";
    }

    internal static class ContentTypes {
        internal const string Json = "application/json";
        internal const string JsonProblem = "application/problem+json";
    }

    // Fully Qualified Names
    internal static class ThirdParty {
        internal static class Classes {
            internal static class FullyQualifiedNames {
                internal const string CancellationToken = "global::System.Threading.CancellationToken";

                internal const string AsParametersAttribute = "global::Microsoft.AspNetCore.Http.AsParametersAttribute";
                internal const string FromBodyAttribute = "global::Microsoft.AspNetCore.Mvc.FromBodyAttribute";
                internal const string FromFormAttribute = "global::Microsoft.AspNetCore.Mvc.FromFormAttribute";
                internal const string FromHeaderAttribute = "global::Microsoft.AspNetCore.Mvc.FromHeaderAttribute";
                internal const string FromQueryAttribute = "global::Microsoft.AspNetCore.Mvc.FromQueryAttribute";
                internal const string FromRouteAttribute = "global::Microsoft.AspNetCore.Mvc.FromRouteAttribute";
            }
        }
    }
}
