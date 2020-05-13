using System;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Nameless.Persistence.NHibernate {
    public sealed class UUIDColumnToStringPropertyUserType : IUserType {
        #region IUserType Members

        public SqlType[] SqlTypes => new [] { SqlTypeFactory.Guid };

        public Type ReturnedType => typeof (string);

        public bool IsMutable => false;

        public object Assemble (object cached, object owner) => cached;

        public object DeepCopy (object value) => value;

        public object Disassemble (object value) => value;

        public new bool Equals (object x, object y) {
            if (x == null) { return y == null; }
            return x.Equals (y);
        }

        public int GetHashCode (object x) {
            return x != null ? x.GetHashCode () : 0;
        }

        public object NullSafeGet (DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
            return NHibernateUtil.String.NullSafeGet (rs, names[0], session, owner);
        }

        public void NullSafeSet (DbCommand cmd, object value, int index, ISessionImplementor session) {
            NHibernateUtil.Guid.NullSafeSet (cmd, Guid.Parse (value.ToString ()), index, session);
        }

        public object Replace (object original, object target, object owner) => original;

        #endregion
    }
}