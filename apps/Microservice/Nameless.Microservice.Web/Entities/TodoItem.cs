namespace Nameless.Microservice.Web.Entities {
    public sealed record TodoItem(Guid Id, string Description, DateTime CreatedAt, DateTime? FinishedAt = null);
}
