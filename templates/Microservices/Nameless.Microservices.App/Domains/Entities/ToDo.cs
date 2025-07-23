namespace Nameless.Microservices.App.Domains.Entities;

public class ToDo {
    public required Guid Id { get; set; }

    public required string Summary { get; set; }

    public required DateTime DueDate { get; set; }

    public DateTime? ConclusionDate { get; set; }
}
