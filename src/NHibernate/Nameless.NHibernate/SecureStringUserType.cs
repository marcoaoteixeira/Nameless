using System.Data.Common;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Security.Cryptography;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Nameless.NHibernate {

    public sealed class SecureStringUserType : IUserType {

        #region Private Read-Only Fields

        private readonly ICryptoProvider _cryptoProvider;

        #endregion

        #region Public Constructors

        public SecureStringUserType() {
            _cryptoProvider = new AesCryptoProvider(Options.Create(CryptoOptions.Default));
        }

        #endregion

        #region IUserType Members

        public SqlType[] SqlTypes => new[] { new StringSqlType() };

        public Type ReturnedType => typeof(string);

        public bool IsMutable => false;

        public object Assemble(object cached, object owner) => cached;

        public object DeepCopy(object value) => value;

        public object Disassemble(object value) => value;

        public new bool Equals(object x, object y) => object.Equals(x, y);

        public int GetHashCode(object x) => SimpleHash.Compute(x);

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
            var value = rs[names.First()];
            if (value == DBNull.Value) { return string.Empty; }

            return _cryptoProvider.Decrypt((string)value);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
            object? parameterValue = DBNull.Value;

            if (value != default) {
                parameterValue = _cryptoProvider.Encrypt((string)value);
            }

            cmd.Parameters[index].Value = parameterValue;
        }

        public object Replace(object original, object target, object owner) => original;

        #endregion
    }
}
