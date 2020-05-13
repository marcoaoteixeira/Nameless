namespace Nameless.Patterns.ChainOfResponsibility {
    /// <summary>
    /// See: https://sourcemaking.com/design_patterns/chain_of_responsibility
    /// </summary>
    /// <typeparam name="T">Type that will by dealt by the handler.</typeparam>
    public interface IHandler<T> where T : class {
        #region Methods

        IHandler<T> Next (IHandler<T> handler);
        object Handle (T request);

        #endregion
    }
}