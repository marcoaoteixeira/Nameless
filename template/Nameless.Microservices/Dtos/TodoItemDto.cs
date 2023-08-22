namespace Nameless.Microservices.Dtos {
    public sealed record TodoItemDto(Guid Id, string Description, DateTime CreatedAt, DateTime? FinishedAt = null) {
        #region Public Properties

        public bool Concluded => FinishedAt.GetValueOrDefault() != DateTime.MinValue;

        #endregion
    }
}
