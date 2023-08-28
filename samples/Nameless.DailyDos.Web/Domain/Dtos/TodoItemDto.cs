namespace Nameless.DailyDos.Web.Domain.Dtos {
    public sealed record TodoItemDto {
        #region Public Properties

        public Guid Id { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? FinishedAt { get; init; }
        public bool Concluded => FinishedAt.GetValueOrDefault() != DateTime.MinValue;

        #endregion
    }
}
