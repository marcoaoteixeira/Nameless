namespace Nameless.MongoDB {
    public interface ICollectionNamingStrategy {
        #region Methods

        string GetCollectionName(Type type);

        #endregion
    }
}
