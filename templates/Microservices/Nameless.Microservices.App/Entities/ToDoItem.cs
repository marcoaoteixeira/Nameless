namespace Nameless.Microservices.App.Entities;

public class ToDoItem {
    public required Guid ID { get; set; }

    public required string Summary { get; set; }

    public required DateTime DueDate { get; set; }

    public DateTime? ConclusionDate { get; set; }
}
