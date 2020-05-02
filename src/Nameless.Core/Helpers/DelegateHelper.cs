using System;
using System.Linq;
using System.Reflection;

namespace Nameless.Helpers {

    /// <summary>
    /// Helper to create a delegate from a <see cref="MethodInfo"/> instance.
    /// </summary>
    public static class DelegateHelper {

        #region Public Static Methods

        /// <summary>
        /// Creates a strongly-typed dynamic delegate that represents a given method on a
        /// given target type.
        /// </summary>
        /// <remarks>
        /// Provided method must be valid for the type provided as targetType.
        /// </remarks>
        /// <typeparam name="T">
        /// Type of the delegate target (first argument) object. Needs to be assignable
        /// from the type provided as targetType parameter.
        /// </typeparam>
        /// <param name="method">Method that the delegate represents.</param>
        /// <returns>
        /// Strongly-typed delegate representing the given method. First argument is the
        /// target of the delegate. Second argument describes call arguments.
        ///  </returns>
        public static Func<T, object[], object> CreateDelegate<T>(MethodInfo method) {
            return CreateDelegate<T>(typeof(T), method);
        }

        /// <summary>
        /// Creates a strongly-typed dynamic delegate that represents a given method on a
        /// given target type.
        /// </summary>
        /// <remarks>
        /// Provided method must be valid for the type provided as targetType.
        /// </remarks>
        /// <typeparam name="T">
        /// Type of the delegate target (first argument) object. Needs to be assignable
        /// from the type provided as targetType parameter.
        /// </typeparam>
        /// <param name="targetType">Delegate target type.</param>
        /// <param name="method">Method that the delegate represents.</param>
        /// <returns>
        /// Strongly-typed delegate representing the given method. First argument is the
        /// target of the delegate. Second argument describes call arguments.
        ///  </returns>
        public static Func<T, object[], object> CreateDelegate<T>(Type targetType, MethodInfo method) {
            var parameters = method.ReturnType == typeof(void)
                ? new[] { targetType }.Concat(method.GetParameters().Select(_ => _.ParameterType)).ToArray()
                : new[] { targetType }.Concat(method.GetParameters().Select(_ => _.ParameterType)).Concat(new[] { method.ReturnType }).ToArray();

            // First fetch the generic form
            var genericHelper = method.ReturnType == typeof(void)
                ? typeof(DelegateHelper).GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(_ => _.Name == nameof(DelegateHelper.BuildAction) && _.GetGenericArguments().Count() == parameters.Length)
                : typeof(DelegateHelper).GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(_ => _.Name == nameof(DelegateHelper.BuildFunction) && _.GetGenericArguments().Count() == parameters.Length);

            // Now supply the type arguments
            var constructedHelper = genericHelper.MakeGenericMethod(parameters);

            // Now call it. The null argument is because it's a static method.
            return (Func<T, object[], object>)constructedHelper.Invoke(null, new object[] { method });
        }

        #endregion Internal Static Methods

        #region Private Static Methods

        private static Func<object, object[], object> BuildFunction<T, TRet>(MethodInfo method) {
            var function = (Func<T, TRet>)method.CreateDelegate(typeof(Func<T, TRet>));

            return (target, parameter) => function((T)target);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, TRet>(MethodInfo method) {
            var function = (Func<T, T1, TRet>)method.CreateDelegate(typeof(Func<T, T1, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, T5, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, T6, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, T6, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, T5, T6, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]
                , (T6)parameter[5]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, T6, T7, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, T6, T7, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, T5, T6, T7, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]
                , (T6)parameter[5]
                , (T7)parameter[6]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, T6, T7, T8, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]
                , (T6)parameter[5]
                , (T7)parameter[6]
                , (T8)parameter[7]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>)method.CreateDelegate(typeof(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]
                , (T6)parameter[5]
                , (T7)parameter[6]
                , (T8)parameter[7]
                , (T9)parameter[8]);
        }

        private static Func<object, object[], object> BuildFunction<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(MethodInfo method) {
            var function = (Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>)method.CreateDelegate(typeof(Func<T, TRet>));

            return (target, parameter) => function((T)target
                , (T1)parameter[0]
                , (T2)parameter[1]
                , (T3)parameter[2]
                , (T4)parameter[3]
                , (T5)parameter[4]
                , (T6)parameter[5]
                , (T7)parameter[6]
                , (T8)parameter[7]
                , (T9)parameter[8]
                , (T10)parameter[9]);
        }

        private static Func<object, object[], object> BuildAction<T>(MethodInfo method) {
            var action = (Action<T>)method.CreateDelegate(typeof(Action<T>));

            return (target, parameter) => {
                action((T)target);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1>(MethodInfo method) {
            var action = (Action<T, T1>)method.CreateDelegate(typeof(Action<T, T1>));

            return (target, parameter) => {
                action((T)target, (T1)parameter[0]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2>(MethodInfo method) {
            var action = (Action<T, T1, T2>)method.CreateDelegate(typeof(Action<T, T1, T2>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3>)method.CreateDelegate(typeof(Action<T, T1, T2, T3>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4, T5>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5, T6>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5, T6>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4, T5, T6>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]
                    , (T6)parameter[5]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5, T6, T7>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5, T6, T7>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4, T5, T6, T7>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]
                    , (T6)parameter[5]
                    , (T7)parameter[6]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5, T6, T7, T8>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5, T6, T7, T8>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4, T5, T6, T7, T8>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]
                    , (T6)parameter[5]
                    , (T7)parameter[6]
                    , (T8)parameter[7]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>)method.CreateDelegate(typeof(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]
                    , (T6)parameter[5]
                    , (T7)parameter[6]
                    , (T8)parameter[7]
                    , (T9)parameter[8]);

                return null;
            };
        }

        private static Func<object, object[], object> BuildAction<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodInfo method) {
            var action = (Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)method.CreateDelegate(typeof(Action<T>));

            return (target, parameter) => {
                action((T)target
                    , (T1)parameter[0]
                    , (T2)parameter[1]
                    , (T3)parameter[2]
                    , (T4)parameter[3]
                    , (T5)parameter[4]
                    , (T6)parameter[5]
                    , (T7)parameter[6]
                    , (T8)parameter[7]
                    , (T9)parameter[8]
                    , (T10)parameter[9]);

                return null;
            };
        }

        #endregion Private Static Methods
    }
}