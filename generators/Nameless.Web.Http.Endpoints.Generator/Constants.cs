namespace Nameless.Web.Http.Endpoints.Generator;

internal static class Constants {
    internal const string PROJECT_NAME = "Nameless.Web.Http.Endpoints.Generator";
    internal const string SELF_REF = "self";
    internal const string BUILDER_REF = "builder";

    internal const string ENDPOINT_EXECUTION_HANDLE_NAME = "HandleAsync";
    internal const string REGISTER_ENDPOINTS_METHOD_NAME = "RegisterAutoEndpoints";
    internal const string MAP_ENDPOINTS_METHOD_NAME = "MapAutoEndpoints";

    internal static class ClassNames {
        internal const string SERVICE_COLLECTION = "IServiceCollection";
        internal const string API_EXPLORER_OPTIONS = "ApiExplorerOptions";
        internal const string API_VERSIONING_OPTIONS = "ApiVersioningOptions";
        internal const string ACTION = "Action";
    }

    internal static class FQN {
        private const string ROOT_NAMESPACE = "Nameless.Web.Http.Endpoints";
        private const string ATTRIBUTES_NAMESPACE = $"{ROOT_NAMESPACE}.Attributes";

        internal const string GENERATED_NAMESPACE = $"{ROOT_NAMESPACE}.Generated";

        internal const string DELETE_HTTP_VERB = $"{ROOT_NAMESPACE}.Delete";
        internal const string GET_HTTP_VERB = $"{ROOT_NAMESPACE}.Get";
        internal const string HEAD_HTTP_VERB = $"{ROOT_NAMESPACE}.Head";
        internal const string OPTIONS_HTTP_VERB = $"{ROOT_NAMESPACE}.Options";
        internal const string PATCH_HTTP_VERB = $"{ROOT_NAMESPACE}.Patch";
        internal const string POST_HTTP_VERB = $"{ROOT_NAMESPACE}.Post";
        internal const string PUT_HTTP_VERB = $"{ROOT_NAMESPACE}.Put";

        internal const string ACTION = "global::System.Action";
        internal const string CANCELLATION_TOKEN = "System.Threading.CancellationToken";
        internal const string OBJECT = "global::System.Object";

        internal const string ENDPOINT_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.EndpointAttribute<THttpVerb>";
        internal const string ENDPOINT_ATTRIBUTE_WITH_ARITY = $"{ATTRIBUTES_NAMESPACE}.EndpointAttribute`1";

        internal const string ACCEPTS_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.AcceptsAttribute";
        internal const string ENDPOINT_FILTER_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.EndpointFilterAttribute";
        internal const string ENDPOINT_GROUPING_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.EndpointGroupingAttribute";
        internal const string PRODUCES_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.ProducesAttribute";
        internal const string PRODUCES_PROBLEM_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.ProducesProblemAttribute";
        internal const string PRODUCES_VALIDATION_PROBLEM_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.ProducesValidationProblemAttribute";
        internal const string VERSION_ATTRIBUTE = $"{ATTRIBUTES_NAMESPACE}.VersionAttribute";

        internal const string ALLOWS_ANONYMOUS_ATTRIBUTE = "Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute";
        internal const string API_EXPLORER_OPTIONS = "global::Asp.Versioning.ApiExplorer.ApiExplorerOptions";
        internal const string API_VERSION = "global::Asp.Versioning.ApiVersion";
        internal const string API_VERSIONING_OPTIONS = "global::Asp.Versioning.ApiVersioningOptions";
        internal const string AUTHORIZE_ATTRIBUTE = "Microsoft.AspNetCore.Authorization.AuthorizeAttribute";
        internal const string DISABLE_HTTP_METRICS_ATTRIBUTE = "Microsoft.AspNetCore.Http.DisableHttpMetricsAttribute";
        internal const string DISABLE_RATE_LIMITING_ATTRIBUTE = "Microsoft.AspNetCore.RateLimiting.DisableRateLimitingAttribute";
        internal const string DISABLE_REQUEST_TIMEOUT_ATTRIBUTE = "Microsoft.AspNetCore.Http.Timeouts.DisableRequestTimeoutAttribute";
        internal const string ENABLE_CORS_ATTRIBUTE = "Microsoft.AspNetCore.Cors.EnableCorsAttribute";
        internal const string ENABLE_RATE_LIMITING_ATTRIBUTE = "Microsoft.AspNetCore.RateLimiting.EnableRateLimitingAttribute";
        internal const string ENDPOINT_DESCRIPTION_ATTRIBUTE = "Microsoft.AspNetCore.Http.EndpointDescriptionAttribute";
        internal const string ENDPOINT_ROUTE_BUILDER = "global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder";
        internal const string ROUTE_GROUP_BUILDER = "global::Microsoft.AspNetCore.Routing.RouteGroupBuilder";
        internal const string ENDPOINT_SUMMARY_ATTRIBUTE = "Microsoft.AspNetCore.Http.EndpointSummaryAttribute";
        internal const string OUTPUT_CACHE_ATTRIBUTE = "Microsoft.AspNetCore.OutputCaching.OutputCacheAttribute";
        internal const string ALLOW_COOKIE_REDIRECT_ATTRIBUTE = "Microsoft.AspNetCore.Authentication.AllowCookieRedirectAttribute";
        internal const string REQUEST_TIMEOUT_ATTRIBUTE = "Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutAttribute";
        internal const string REQUIRE_ANTIFORGERY_TOKEN_ATTRIBUTE = "Microsoft.AspNetCore.Antiforgery.RequireAntiforgeryTokenAttribute";
        internal const string SERVICE_COLLECTION = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

        internal const string AS_PARAMETERS_ATTRIBUTE = "Microsoft.AspNetCore.Http.AsParametersAttribute";
        internal const string FROM_BODY_ATTRIBUTE = "Microsoft.AspNetCore.Mvc.FromBodyAttribute";
        internal const string FROM_HEADER_ATTRIBUTE = "Microsoft.AspNetCore.Mvc.FromHeaderAttribute";
        internal const string FROM_QUERY_ATTRIBUTE = "Microsoft.AspNetCore.Mvc.FromQueryAttribute";
        internal const string FROM_ROUTE_ATTRIBUTE = "Microsoft.AspNetCore.Mvc.FromRouteAttribute";

        internal const string AS_PARAMETERS = "global::Microsoft.AspNetCore.Http.AsParameters";
        internal const string FROM_BODY = "global::Microsoft.AspNetCore.Mvc.FromBody";
        internal const string FROM_HEADER = "global::Microsoft.AspNetCore.Mvc.FromHeader";
        internal const string FROM_QUERY = "global::Microsoft.AspNetCore.Mvc.FromQuery";
        internal const string FROM_ROUTE = "global::Microsoft.AspNetCore.Mvc.FromRoute";
        internal const string FROM_SERVICES = "global::Microsoft.AspNetCore.Mvc.FromServices";
    }

    internal static class HttpVerbs {
        internal const string DELETE = "Delete";
        internal const string GET = "Get";
        internal const string HEADER = "Header";
        internal const string OPTIONS = "Options";
        internal const string PATCH = "Patch";
        internal const string POST = "Post";
        internal const string PUT = "Put";
    }
}
