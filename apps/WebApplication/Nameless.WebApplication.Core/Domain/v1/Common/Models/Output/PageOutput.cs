namespace Nameless.WebApplication.Domain.v1.Common.Models.Output {

    public sealed class PageOutput<T> {

        #region Public Properties

        public T[] Items { get; set; } = Array.Empty<T>();
        public int Index { get; set; }

        public int Number => Index + 1;

        public int Size { get; set; }
        public int Length => Items.Length;

        public int PageCount => (int)Math.Ceiling(Total / (decimal)Size);

        public int Total { get; set; }

        public bool HasNext => Number < PageCount;

        public bool HasPrevious => Number > 1;

        #endregion
    }
}
