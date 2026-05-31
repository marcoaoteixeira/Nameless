namespace Nameless.Web.Http.Endpoints.Generator;

public static class Constants {
    public static class Project {
        public const string Name = "Nameless Auto Endpoints Generator";
        public const string Namespace = "Nameless.Web.Http.Endpoints.Generator";
        public const string RegistrationClassName = "AutoEndpointsRegistrationExtensions";
        public const string RegisterEndpointsMethodName = "RegisterAutoEndpoints";
        public const string MapEndpointsMethodName = "MapAutoEndpoints";
    }

    public static class EndpointClass {
        public const string HandlerMethodName = "HandleAsync";
    }

    public static class EndpointGroupClass {
        public const string ReservedName = "__Synthetic__";
    }

    public static class ContentTypes {
        public const string Json = "application/json";
        public const string JsonProblem = "application/problem+json";
    }

    public static class Simple {
        private const string ATTR_NS = "Nameless.Web.Http.Endpoints.Attributes";

        public const string EndpointAttributeWithArity = $"{ATTR_NS}.EndpointAttribute`1";
        public const string EndpointGroupAttribute = $"{ATTR_NS}.EndpointGroupAttribute";
    }

    public static class ClassNames {
        public const string ServiceCollection = "IServiceCollection";
        public const string ApiExplorerOptions = "ApiExplorerOptions";
        public const string ApiVersioningOptions = "ApiVersioningOptions";
        public const string Action = "Action";
        public const string EndpointAttribute = "EndpointAttribute<THttpVerb>";
        public const string EndpointGroupAttribute = "EndpointGroupAttribute";
    }

    // Fully Qualified Names
    public static class FQN {
        public static class Nameless {
            private const string ROOT_NS = "global::Nameless.Web.Http.Endpoints";
            private const string ATTRIBUTES_NS = "global::Nameless.Web.Http.Endpoints.Attributes";

            public const string HttpVerbDelete = $"{ROOT_NS}.Delete";
            public const string HttpVerbGet = $"{ROOT_NS}.Get";
            public const string HttpVerbHead = $"{ROOT_NS}.Head";
            public const string HttpVerbOptions = $"{ROOT_NS}.Options";
            public const string HttpVerbPatch = $"{ROOT_NS}.Patch";
            public const string HttpVerbPost = $"{ROOT_NS}.Post";
            public const string HttpVerbPut = $"{ROOT_NS}.Put";

            public const string SunsetMetadata = $"{ATTRIBUTES_NS}.Versioning.SunsetMetadata";
        }

        public static class System {
            private const string ROOT_NS = "global::System";
            private const string THREADING_NS = $"{ROOT_NS}.Threading";
            private const string GENERIC_NS = $"{ROOT_NS}.Collections.Generic";

            public const string Object = $"{ROOT_NS}.Object";
            public const string Action = $"{ROOT_NS}.Action";

            public const string CancellationToken = $"{THREADING_NS}.CancellationToken";
            public const string Task = $"{THREADING_NS}.Tasks.Task";

            public const string Dictionary = $"{GENERIC_NS}.Dictionary";
        }

        public static class Microsoft {
            private const string ROOT_NS = "global::Microsoft";
            private const string ASPNETCORE_NS = $"{ROOT_NS}.AspNetCore";
            private const string EXTENSIONS_NS = $"{ROOT_NS}.Extensions";
            private const string OPENAPI_NS = $"{ROOT_NS}.OpenApi";

            public const string AuthorizeAttribute = $"{ASPNETCORE_NS}.Authorization.AuthorizeAttribute";
            public const string DisableCorsAttribute = $"{ASPNETCORE_NS}.Cors.DisableCorsAttribute";

            public const string AsParametersAttribute = $"{ASPNETCORE_NS}.Http.AsParametersAttribute";
            public const string FromBodyAttribute = $"{ASPNETCORE_NS}.Mvc.FromBodyAttribute";
            public const string FromFormAttribute = $"{ASPNETCORE_NS}.Mvc.FromFormAttribute";
            public const string FromHeaderAttribute = $"{ASPNETCORE_NS}.Mvc.FromHeaderAttribute";
            public const string FromQueryAttribute = $"{ASPNETCORE_NS}.Mvc.FromQueryAttribute";
            public const string FromRouteAttribute = $"{ASPNETCORE_NS}.Mvc.FromRouteAttribute";

            public const string AsParameters = $"{ASPNETCORE_NS}.Http.AsParameters";
            public const string FromBody = $"{ASPNETCORE_NS}.Mvc.FromBody";
            public const string FromForm = $"{ASPNETCORE_NS}.Mvc.FromBody";
            public const string FromHeader = $"{ASPNETCORE_NS}.Mvc.FromHeader";
            public const string FromQuery = $"{ASPNETCORE_NS}.Mvc.FromQuery";
            public const string FromRoute = $"{ASPNETCORE_NS}.Mvc.FromRoute";
            public const string FromServices = $"{ASPNETCORE_NS}.Mvc.FromServices";

            
            public const string EndpointRouteBuilder = $"{ASPNETCORE_NS}.Routing.IEndpointRouteBuilder";
            public const string EndpointConventionBuilder = $"{ASPNETCORE_NS}.Builder.IEndpointConventionBuilder";

            public const string RouteGroupBuilder = $"{ASPNETCORE_NS}.Routing.RouteGroupBuilder";

            public const string ServiceCollection = $"{EXTENSIONS_NS}.DependencyInjection.IServiceCollection";

            public const string OpenApiExtension = $"{OPENAPI_NS}.IOpenApiExtension";
            public const string JsonNodeExtension = $"{OPENAPI_NS}.JsonNodeExtension";
        }

        public static class AspVersioning {
            private const string ROOT_NS = "global::Asp.Versioning";

            public const string ApiVersion = $"{ROOT_NS}.ApiVersion";
            public const string ApiVersioningOptions = $"{ROOT_NS}.ApiVersioningOptions";
            public const string ApiExplorerOptions = $"{ROOT_NS}.ApiExplorer.ApiExplorerOptions";
        }
    }
}
