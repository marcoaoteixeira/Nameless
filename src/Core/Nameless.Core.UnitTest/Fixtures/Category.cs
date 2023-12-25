namespace Nameless.Fixtures {
    public class Category {
        public string? Name { get; set; }
        public List<Category> Children { get; } = [];
    }
}
