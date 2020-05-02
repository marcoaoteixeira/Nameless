using Castle.DynamicProxy;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Defines methods to intercept actions of <see cref="global::NHibernate.ISession"/>.
    /// </summary>
	public interface ISessionInterceptor : IInterceptor { }
}