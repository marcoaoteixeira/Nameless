using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nameless.Orm {

    /// <summary>
    /// Represents a directive parameter.
    /// </summary>
    public sealed class NameValueParameter {

        #region Public Properties

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="NameValueParameter" />.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public NameValueParameter (string name, object value) {
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            Name = name;
            Value = value;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Retrieves the typed parameter value. If defined value is not of the type <typeparamref name="T" />
        /// tries to change type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <returns>The typed parameter value.</returns>
        public T GetValue<T> () {
            if (Value is T) { return (T) Value; }

            return (T) Convert.ChangeType (Value, typeof (T));
        }

        /// <summary>
        /// Checks if the current instance is equals to the specified instance.
        /// </summary>
        /// <param name="obj">The <see cref="NameValueParameter"/> instance to check.</param>
        /// <returns><c>true</c> if equals; otherwise <c>false</c></returns>
        public bool Equals (NameValueParameter obj) {
            return obj != null && string.Equals (obj.Name, Name, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion Public Methods

        #region Public Override Methods

        /// <inheritdoc />
        public override bool Equals (object obj) {
            return Equals (obj as NameValueParameter);
        }

        /// <inheritdoc />
        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Name ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion Public Override Methods
    }

    /// <summary>
    /// Represents a set of directive parameters.
    /// </summary>
    /// <typeparam name="NameValueParameter"></typeparam>
    public sealed class NameValueParameterSet : IEnumerable<NameValueParameter> {

        #region Private Read-Only Fields

        private readonly ISet<NameValueParameter> _items;

        #endregion Private Read-Only Fields

        #region Public Properties

        public NameValueParameter this [string name] {
            get { return GetParameter (name); }
            set { SetParameter (name, value); }
        }

        #endregion Public Properties

        #region Public Constructors

        public NameValueParameterSet (IEnumerable<NameValueParameter> collection) {
            _items = new HashSet<NameValueParameter> (collection ?? Enumerable.Empty<NameValueParameter> ());
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Adds a new parameter to the set.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns><c>true</c> if added; otherwise <c>false</c></returns>
        public bool Add (string name, object value) {
            return _items.Add (new NameValueParameter (name, value));
        }
        /// <summary>
        /// Adds a new parameter to the set.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if added; otherwise <c>false</c></returns>
        public bool Add (NameValueParameter parameter) {
            return _items.Add (parameter);
        }
        /// <summary>
        /// Removes a parameter from the set.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <returns><c>true</c> if removed; otherwise <c>false</c></returns>
        public bool Remove (string name) {
            var item = _items.SingleOrDefault (_ => string.Equals (_.Name, name, StringComparison.CurrentCultureIgnoreCase));

            return (item != null) ?
                _items.Remove (item) :
                false;
        }
        /// <summary>
        /// Removes a parameter from the set.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if removed; otherwise <c>false</c></returns>
        public bool Remove (NameValueParameter parameter) {
            return _items.Remove (parameter);
        }

        #endregion Public Methods

        #region Private Methods

        private NameValueParameter GetParameter (string name) {
            return _items.SingleOrDefault (_ => string.Equals (_.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        private void SetParameter (string name, NameValueParameter parameter) {
            var currentParameter = GetParameter (name);

            if (currentParameter != null) {
                _items.Remove (currentParameter);
            }

            _items.Add (parameter);
        }

        #endregion Private Methods

        #region IEnumerable Members

        /// <inheritdoc />
        public IEnumerator<NameValueParameter> GetEnumerator () {
            return _items.GetEnumerator ();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator () {
            return ((IEnumerable) _items).GetEnumerator ();
        }

        #endregion IEnumerable Members
    }
}