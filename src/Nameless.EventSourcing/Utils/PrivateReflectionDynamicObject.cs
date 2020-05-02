using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Nameless.EventSourcing.Utils {

    internal class PrivateReflectionDynamicObject : DynamicObject {

        #region Private Constants

        private const BindingFlags CurrentBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        #endregion Private Constants

        #region Private Static Read-Only Fields

        private static readonly ConcurrentDictionary<int, CompiledMethodInfo> Cache = new ConcurrentDictionary<int, CompiledMethodInfo> ();

        #endregion Private Static Read-Only Fields

        #region Public Properties

        internal object RealObject { get; set; }

        #endregion Public Properties

        #region Public Override Methods

        public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result) {
            var methodName = binder.Name;
            var type = RealObject.GetType ();

            var hash = 13;
            unchecked {
                hash += type.GetHashCode () * 7;
                hash += methodName.GetHashCode () * 7;
            }

            var argumentTypes = new Type[args.Length];
            for (var idx = 0; idx < args.Length; idx++) {
                var argumentType = args[idx].GetType ();
                argumentTypes[idx] = argumentType;
                unchecked {
                    hash += argumentType.GetHashCode () * 7;
                }
            }
            var method = Cache.GetOrAdd (hash, _ => {
                var member = GetMember (type, methodName, argumentTypes);
                return member == null ? null : new CompiledMethodInfo (member, type);
            });
            result = method?.Invoke (RealObject, args);

            return true;
        }

        #endregion Public Override Methods

        #region Private Static Methods

        private static MethodInfo GetMember (Type type, string name, Type[] argumentTypes) {
            if (type == null) { return null; }

            var member = type
                .GetMethods (CurrentBindingFlags)
                .FirstOrDefault (method => method.Name == name && method.GetParameters ()
                                                                        .Select (parameter => parameter.ParameterType)
                                                                        .SequenceEqual (argumentTypes));

            if (member != null) { return member; }

            return GetMember (type.GetTypeInfo ().BaseType, name, argumentTypes);
        }

        #endregion Private Static Methods
    }
}