namespace Nameless.DailyDos.Web.Domain.Entities {
    public sealed record TodoItem {
        #region Public Properties

        public Guid Id { get; init; }
        public string Description { get; init; } = null!;
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public DateTime? FinishedAt { get; init; }

        #endregion

        #region Public Constructors

        public TodoItem() {
            Id = Guid.NewGuid();
        }

        public TodoItem(Guid id) {
            Id = id;
        }

        #endregion
    }
}
