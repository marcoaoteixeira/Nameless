namespace Nameless.Utils.UnitTests {
    public class User {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public class Order {
        public int ID { get; set; }
        public string? Name { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
    }

    public class Product {
        public int ID { get; set; }
        public string? Name { get; set; }
    }

    public class SeededOrder : Order {
        public SeededOrder() {
            Products.Add(new() { ID = 1, Name = "Bike" });
            Products.Add(new() { ID = 2, Name = "Helmet" });
        }
    }
}