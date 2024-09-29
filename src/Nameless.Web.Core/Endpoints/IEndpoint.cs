using Asp.Versioning.Builder;

namespace Nameless.Web.Endpoints;

public interface IEndpoint {
    string HttpMethod { get; }

    string RoutePattern { get; }
    
    string Name { get; }
    
    string Description { get; }
    
    string Summary { get; }

    string GroupName { get; }

    string[] Tags { get; }

    AcceptMetadata[] Accepts { get; }

    int Version { get; }

    bool Deprecated { get; }

    int MapToVersion { get; }

    ProducesMetadata[] Produces { get; }

    Delegate CreateDelegate();
}