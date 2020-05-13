using System;
using System.Data.Common;
using Nameless.Security.Cryptography;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Nameless.Persistence.NHibernate {
    public sealed class SecureStringUserType : IUserType {
        #region IUserType Members

        public SqlType[] SqlTypes => new [] { new StringSqlType () };

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
            var value = rs[names[0]];
            return value != DBNull.Value ? AesCryptoProvider.Instance.Decrypt ((string) value) : null;
        }

        public void NullSafeSet (DbCommand cmd, object value, int index, ISessionImplementor session) {
            if (value != null) {
                var newValue = AesCryptoProvider.Instance.Encrypt ((string) value);
                cmd.Parameters[index].Value = newValue;
            }
        }

        public object Replace (object original, object target, object owner) => original;

        #endregion
    }
}