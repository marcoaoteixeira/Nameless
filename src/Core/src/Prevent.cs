using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Nameless {
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Prevent {

        #region Public Static Methods

        /// <summary>
        /// Ensures that the parameter value is not <c>null</c>.
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="parameterValue"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void ParameterNull (object parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            if (parameterValue == null) {
                throw new ArgumentNullException (parameterName);
            }
        }

        /// <summary>
        /// Ensures that the parameter value cannot be <c>null</c>, empty or white spaces.
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="parameterValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="parameterValue"/> is empty or white space.
        /// </exception>
        [DebuggerStepThrough]
        public static void ParameterNullOrWhiteSpace (string parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            ParameterNull (parameterValue, parameterName);

            if (string.IsNullOrWhiteSpace (parameterValue)) {
                throw new ArgumentException ($"Parameter '{parameterName}' cannot be null, empty or white spaces.");
            }
        }

        /// <summary>
        /// Ensures that the parameter value cannot be <c>null</c>, empty collection.
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="parameterValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="parameterValue"/> is empty.
        /// </exception>
        [DebuggerStepThrough]
        public static void ParameterNullOrEmpty<T> (T[] parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            ParameterNull (parameterValue, parameterName);

            if (!parameterValue.Any ()) {
                throw new ArgumentException ($"Parameter '{parameterName}' empty.");
            }
        }

        /// <summary>
        /// Ensure that the type is assignable from the inherit type.
        /// </summary>
        /// <param name="baseType">The type.</param>
        /// <param name="specificType">The inherit type.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="baseType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="specificType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="baseType"/> is not assignable from <paramref name="specificType"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static void ParameterTypeNotAssignableFrom (Type baseType, Type specificType) {
            ParameterNull (baseType, nameof (baseType));
            ParameterNull (specificType, nameof (specificType));

            if (!baseType.GetTypeInfo ().IsAssignableFrom (specificType.GetTypeInfo ())) {
                throw new ArgumentException ($"The specified type ({specificType}) must be assignable to {baseType}");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if range was not matched.
        /// </summary>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        [DebuggerStepThrough]
        public static void ParameterOutOfRange (int minimumValue, int maximumValue, int parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            if (parameterValue < minimumValue || parameterValue > maximumValue) {
                throw new ArgumentOutOfRangeException (parameterName, parameterValue, $"Range: {minimumValue} to {maximumValue}");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if range was not matched.
        /// </summary>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        [DebuggerStepThrough]
        public static void ParameterOutOfRange (long minimumValue, long maximumValue, long parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            if (parameterValue < minimumValue || parameterValue > maximumValue) {
                throw new ArgumentOutOfRangeException (parameterName, parameterValue, $"Range: {minimumValue} to {maximumValue}");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if range was not matched.
        /// </summary>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        [DebuggerStepThrough]
        public static void ParameterOutOfRange (decimal minimumValue, decimal maximumValue, decimal parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            if (parameterValue < minimumValue || parameterValue > maximumValue) {
                throw new ArgumentOutOfRangeException (parameterName, parameterValue, $"Range: {minimumValue} to {maximumValue}");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if range was not matched.
        /// </summary>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        [DebuggerStepThrough]
        public static void ParameterOutOfRange (double minimumValue, double maximumValue, double parameterValue, string parameterName) {
            CheckParameterName (parameterName);

            if (parameterValue < minimumValue || parameterValue > maximumValue) {
                throw new ArgumentOutOfRangeException (parameterName, parameterValue, $"Range: {minimumValue} to {maximumValue}");
            }
        }

        #endregion Public Static Methods

        #region Private Static Methods

        [DebuggerStepThrough]
        private static void CheckParameterName (string parameterName) {
            if (string.IsNullOrWhiteSpace (parameterName)) {
                throw new ArgumentException ($"Parameter '{nameof (parameterName)}' cannot be null, empty or white spaces.");
            }
        }

        #endregion Private Static Methods
    }
}