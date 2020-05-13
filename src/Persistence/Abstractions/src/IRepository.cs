namespace Nameless.Persistence {

    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IRepository : IPersister, IQuerier, IDirectiveExecutor { }
}