using System;

namespace Nameless.Orm.NHibernate {
    public class ApplicationContext : IInterceptorContext {
        #region Public Properties

        public bool IsSystemAdministrator { get; set; }
        public Guid Owner { get; set; }

        #endregion

        #region IInterceptorContext Members

        public object State { get; }

        #endregion
    }
}
