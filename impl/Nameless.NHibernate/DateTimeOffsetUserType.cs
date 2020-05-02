using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Nameless.NHibernate {
    /// <summary>
    /// <see cref="DateTimeOffset"/> implementation of <see cref="IUserType"/>
    /// </summary>
    public class DateTimeOffsetUserType : IUserType, IParameterizedType {
        #region Public Constructors

        public DateTimeOffsetUserType () { }

        public DateTimeOffsetUserType (TimeSpan offset) {
            Offset = offset;
        }

        #endregion

        #region IUserType Members
        public TimeSpan Offset { get; private set; }

        public Type ReturnedType => typeof (DateTimeOffset);

        public SqlType[] SqlTypes => new [] { new SqlType (DbType.DateTime) };

        public object NullSafeGet (DbDataReader dr, string[] names, ISessionImplementor session, object owner) {
            Prevent.ParameterNull (dr, nameof (dr));
            Prevent.ParameterNullOrEmpty (names, nameof (names));

            var name = names[0];
            var index = dr.GetOrdinal (name);

            if (dr.IsDBNull (index)) { return null; }

            try {
                DateTime storedTime;
                try {
                    var dbValue = Convert.ToDateTime (dr[index]);
                    storedTime = new DateTime (dbValue.Year, dbValue.Month, dbValue.Day, dbValue.Hour, dbValue.Minute, dbValue.Second);
                } catch (Exception ex) {
                    throw new FormatException ($"Input string '{dr[index]}' was not in the correct format.", ex);
                }
                return new DateTimeOffset (storedTime, Offset);
            } catch (InvalidCastException ex) {
                throw new ADOException ($"Could not cast the value in field {names[0]} of type {dr[index].GetType ().Name} to the Type {GetType ().Name}. Please check to make sure that the mapping is correct and that your DataProvider supports this Data Type.", ex);
            }
        }

        public void NullSafeSet (DbCommand cmd, object value, int index, ISessionImplementor session) {
            Prevent.ParameterNull (cmd, nameof (cmd));

            if (value != null) {
                var dateTimeOffset = (DateTimeOffset) value;
                var paramVal = dateTimeOffset.ToOffset (Offset).DateTime;

                cmd.Parameters[index].Value = paramVal;
            } else { NHibernateUtil.DateTime.NullSafeSet (cmd, null, index, session); }
        }

        public object Assemble (object cached, object owner) => cached;

        public object DeepCopy (object value) => value;

        public object Disassemble (object value) => value;

        public new bool Equals (object x, object y) {
            if (x == null) { return y == null; }
            return x.Equals (y);
        }

        public int GetHashCode (object x) {
            if (x == null) { return 0; }
            return x.GetHashCode ();
        }

        public bool IsMutable => false;

        public object Replace (object original, object target, object owner) => original;

        public int Compare (object x, object y) => ((DateTimeOffset) x).CompareTo ((DateTimeOffset) y);

        #endregion

        #region IParameterizedType Members

        public void SetParameterValues (System.Collections.Generic.IDictionary<string, string> parameters) {
            if (parameters != null && parameters.TryGetValue ("Offset", out string offset)) {
                Offset = TimeSpan.Parse (offset);
            }
        }

        #endregion
    }
}