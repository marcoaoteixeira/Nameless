namespace Nameless.Web.Endpoints.Definitions;

public record AcceptMetadata(Type RequestType, bool IsOptional, string[] ContentTypes);

public record ProduceMetadata(Type ResponseType, int StatusCode, string[] ContentTypes);
