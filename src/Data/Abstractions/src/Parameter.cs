using System;
using System.Data;

namespace Nameless.Data {

    /// <summary>
    /// Represents a command parameter.
    /// </summary>
    public sealed class Parameter {

        #region Public Properties

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public DbType Type { get; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the parameter direction.
        /// </summary>
        public ParameterDirection Direction { get; }

        #endregion

        #region Private Constructors

        private Parameter (string name, object value, DbType type, ParameterDirection direction) {
            Name = name;
            Value = value;
            Type = type;
            Direction = direction;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates an input parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="type">The parameter type (default: <see cref="DbType.String"/>.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter CreateInputParameter (string name, object value, DbType type = DbType.String) {
            return new Parameter (name, value, type, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates an output parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="type">The parameter type (default: <see cref="DbType.String"/>.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter CreateOutputParameter (string name, object value, DbType type = DbType.String) {
            return new Parameter (name, value, type, ParameterDirection.Output);
        }

        /// <summary>
        /// Creates a return value parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="type">The parameter type (default: <see cref="DbType.String"/>.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter CreateReturnValueParameter (string name, object value, DbType type = DbType.String) {
            return new Parameter (name, value, type, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// Parses a value.
        /// </summary>
        /// <typeparam name="T">Type of the input parameter.</typeparam>
        /// <param name="value">The input value.</param>
        /// <returns>The object parsed.</returns>
        public static object Parse<T> (T value) {
            object result = null;
            switch (System.Type.GetTypeCode (typeof (T))) {
                case TypeCode.Object:
                    result = value;
                    break;

                case TypeCode.Boolean:
                    result = Convert.ToBoolean (value);
                    break;

                case TypeCode.Char:
                    result = Convert.ToChar (value);
                    break;

                case TypeCode.SByte:
                    result = Convert.ToSByte (value);
                    break;

                case TypeCode.Byte:
                    result = Convert.ToByte (value);
                    break;

                case TypeCode.Int16:
                    result = Convert.ToInt16 (value);
                    break;

                case TypeCode.UInt16:
                    result = Convert.ToUInt16 (value);
                    break;

                case TypeCode.Int32:
                    result = Convert.ToInt32 (value);
                    break;

                case TypeCode.UInt32:
                    result = Convert.ToUInt32 (value);
                    break;

                case TypeCode.Int64:
                    result = Convert.ToInt64 (value);
                    break;

                case TypeCode.UInt64:
                    result = Convert.ToUInt64 (value);
                    break;

                case TypeCode.Single:
                    result = Convert.ToSingle (value);
                    break;

                case TypeCode.Double:
                    result = Convert.ToDouble (value);
                    break;

                case TypeCode.Decimal:
                    result = Convert.ToDecimal (value);
                    break;

                case TypeCode.DateTime:
                    result = Convert.ToDateTime (value);
                    break;

                case TypeCode.String:
                    result = Convert.ToString (value);
                    break;
            }
            return result ?? DBNull.Value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance and another specified <see cref="Parameter"/> object havethe same value.
        /// </summary>
        /// <param name="obj">The <see cref="Parameter"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of the value parameter is the same as the value of this instance;
        /// otherwise, <c>false</c>. If value is null, the method returns <c>false</c>.
        /// </returns>
        public bool Equals (Parameter obj) {
            return obj != null && obj.Name == Name;
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override bool Equals (object obj) {
            return Equals (obj as Parameter);
        }

        /// <inheritdoc />
        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Name ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        /// <inheritdoc />
        public override string ToString () {
            return $"({Direction}) {Name}: {Value}";
        }

        #endregion
    }
}