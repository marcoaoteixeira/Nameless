using System;

namespace Nameless.IoC {

    /// <summary>
    /// Class used to inject parameter in service when requested.
    /// </summary>
    public sealed class Parameter {

        #region Public Properties

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public Type Type { get; }

        #endregion

        #region Private Constructors

        private Parameter (string name, object value, Type type) {
            Name = name;
            Value = value;
            Type = type ?? value?.GetType ();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a new <see cref="Parameter"/>.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter Create (string name, object value) {
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            return new Parameter (name: name, value: value, type: null);
        }
        /// <summary>
        /// Creates a new <see cref="Parameter"/>.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter Create (Type type, object value) {
            Prevent.ParameterNull (type, nameof (type));

            return new Parameter (name: null, value: value, type: type);
        }
        /// <summary>
        /// Creates a new <see cref="Parameter"/>.
        /// </summary>
        /// <typeparam name="TParameter">The parameter type.</typeparam>
        /// <param name="value">The parameter value.</param>
        /// <returns>An instance of <see cref="Parameter"/>.</returns>
        public static Parameter Create<TParameter> (object value) {
            return new Parameter (name: null, value: value, type: typeof (TParameter));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The specified <see cref="Parameter"/>.</param>
        /// <returns><c>true</c> if is equals; otherwise, <c>false</c>.</returns>
        public bool Equals (Parameter obj) {
            return obj != null &&
                obj.Name == Name &&
                obj.Value == Value &&
                obj.Type == Type;
        }

        #endregion Public Methods

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
                hash += (Value != null ? Value.GetHashCode () : 0) * 7;
                hash += (Type != null ? Type.GetHashCode () : 0) * 7;
            }
            return hash;
        }

        /// <inheritdoc />
        public override string ToString () {
            if (Name == null && Value == null && Type == null) { return null; }

            var hasName = !string.IsNullOrWhiteSpace (Name);
            var typeName = Type == null
                ? Value?.GetType ().Name
                : Type.Name;

            return hasName ?
                $"{Name} => {Value} ({typeName})" :
                $"{typeName} => {Value}";
        }

        #endregion
    }
}