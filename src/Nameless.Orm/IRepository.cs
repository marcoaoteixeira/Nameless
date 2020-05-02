namespace Nameless.Orm {

    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IRepository : IPersister, IQuerier, IDirectiveExecutor { }
}