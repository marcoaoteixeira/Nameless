namespace Nameless.Orm.NHibernate {
    public interface IInterceptorContext {
        #region Properties

        object State { get; }

        #endregion
    }
}