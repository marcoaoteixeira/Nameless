using Nameless.Validation;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo;

[Validate]
public record CreateToDoInput {
    public string? Summary { get; set; }
    public DateTime? DueDate { get; set; }
}