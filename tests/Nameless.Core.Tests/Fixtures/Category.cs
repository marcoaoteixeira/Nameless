namespace Nameless.Fixtures;

public class Category {
    public string Name { get; set; }
    public List<Category> Children { get; } = [];

    public void DoSomething(int value) {
        Console.WriteLine(value);
    }
}