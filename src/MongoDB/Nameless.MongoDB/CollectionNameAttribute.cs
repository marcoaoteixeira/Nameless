namespace Nameless.MongoDB;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CollectionNameAttribute : Attribute {
    public string Name { get; }

    public CollectionNameAttribute(string name) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
    }
}