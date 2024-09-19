namespace Nameless.MongoDB;

public interface ICollectionNamingStrategy {
    string GetCollectionName(Type type);
}