# Idea: Minimal API Generator

## Overview
This feature aims to introduce a code generator leveraging modern .NET and C# capabilities (such as source generators and attributes) to simplify the creation of Minimal API endpoints.

The generator will allow developers to define endpoints declaratively by decorating classes with attributes, where each class represents a single endpoint and encapsulates its behavior.

## Motivation
In current ASP.NET Core Minimal API implementations, endpoints are typically registered during application startup (e.g., in `Program.cs`) using `Map*` extension methods on `IEndpointRouteBuilder`.

This approach presents several drawbacks:

- **Tight coupling to application startup**: Endpoint definitions are centralized in the configuration phase, reducing modularity.
- **Limited testability**: Handlers are often implemented as static delegates or lambdas, making them difficult to test in isolation.
- **Testing overhead**: Verifying endpoint behavior usually requires spinning up a full application using tools like WebApplicationFactory, along with an HTTP client—introducing unnecessary complexity and boilerplate.

## Proposed Solution
Introduce a pattern where each endpoint is represented by a dedicated class, for example:

- A class defines the endpoint contract and behavior.
- A standard method (e.g., HandleAsync) encapsulates the request handling logic.
- Attributes are used to describe metadata such as route, HTTP method, and other configurations.

The source generator will:

- Discover all classes annotated with endpoint-related attributes.
- Automatically generate the necessary wiring to register these endpoints with the ASP.NET Core pipeline.
- Eliminate the need for manual `Map*` calls in `Program.cs`.

## Benefits
- Improved testability: Endpoint classes can be instantiated directly, allowing unit testing without requiring a full application host.
- Better separation of concerns: Endpoint logic is decoupled from application configuration.
- Reduced boilerplate: Automatic registration removes repetitive mapping code.
- Enhanced maintainability: Endpoints become self-contained, discoverable, and easier to reason about.
- Alignment with modern .NET practices: Leverages source generators and attribute-based programming for a cleaner developer experience.

## Core Capabilities
### 1. Versioning
Endpoints must support versioning through attributes, enabling multiple versions of the same endpoint to coexist.

- Version metadata should be declarative
- Routing should support version placeholders (e.g., v{version})

### 2. Endpoint Filters
Support integration with IEndpointFilter to allow cross-cutting concerns such as logging, validation, and authorization.

- Filters should be attachable via attributes
- Multiple filters per endpoint must be supported
- Execution order should be deterministic

### 3. Endpoint Grouping
Endpoints can be logically grouped to:

- Share route prefixes
- Share metadata (e.g., version sets, policies)
- Improve organization and maintainability

## Attribute Model
Existing Attributes (from ASP.NET Core)

The generator must recognize and apply existing attributes from the Microsoft.AspNetCore.Mvc ecosystem, including:

- [EndpointSummary]
- [EndpointDescription]
- [Authorize]
- [EnableCors]
- [EnableRateLimiting]
- [OutputCache]
- [RequestTimeout]
- [DisableHttpMetrics]
- [AllowAnonymous]

These should be seamlessly propagated into the generated endpoint configuration.

### Custom Attributes (to be implemented)

The following attributes must be introduced and supported by the generator:

Filters

```csharp
[Filter<RequestLoggingFilter>]
```
- Attaches an IEndpointFilter implementation to the endpoint
- Must support multiple usages per endpoint

### Response Metadata

```csharp
[Produces<User>(statusCode: 200, contentType: "application/json")]
[ProducesProblem(statusCode: 500, contentType: "application/json")]
[ProducesValidationProblem(statusCode: 400, contentType: "application/json")]
```
- Defines response types and status codes
- Must integrate with OpenAPI/Swagger generation
- Should align with existing Produces* conventions

### Versioning

```csharp
[Version("1.0.0")]
```
- Declares the version(s) supported by the endpoint
- Must integrate with route templates and grouping

### Request Handling

```csharp
[Accepts<object>]
[UseAntiforgery]
```
- Accepts<T> defines the expected request body type
- UseAntiforgery enables antiforgery validation

### Grouping

```csharp
[Group(
    Name = "Users",
    Prefix = "/api/v{version:apiVersion}/users",
    Versions = ["1.0.0", "2.0.0"]
)]
```
- Defines a logical group for endpoints
- Applies shared route prefix and version set
- Group-level metadata should cascade to endpoints unless overridden

## Expected Outcome

Developers will be able to define endpoints like this:

```csharp
[HttpGet("/{id}")]
[Version("1.0.0")]
[Group(Name = "Users", Prefix = "/api/v{version:apiVersion}/users")]
[Produces<User>(200)]
public class GetUserEndpoint
{
    public async Task<IResult> HandleAsync(int id, IUserService service)
    {
        var user = await service.GetByIdAsync(id);
        return Results.Ok(user);
    }
}

The generator should:

- Resolve the full route: /api/v{version:apiVersion}/users/{id}
- Register the endpoint for version 1.0.0
- Attach metadata (Produces, Summary, etc.)
- Apply filters (if any)
- Integrate with OpenAPI
- Require zero manual registration in Program.cs