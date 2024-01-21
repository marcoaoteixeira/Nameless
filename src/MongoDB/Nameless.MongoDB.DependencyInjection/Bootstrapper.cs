using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.Serialization;

namespace Nameless.MongoDB.DependencyInjection {
    public sealed class Bootstrapper {
        #region Private Read-Only Fields

        private readonly ILogger _logger;
        private readonly Type[] _mappings;

        #endregion

        #region Public Constructors

        public Bootstrapper(Type[] mappings)
            : this(mappings, NullLogger.Instance) { }

        public Bootstrapper(Type[] mappings, ILogger logger) {
            _mappings = Guard.Against.Null(mappings, nameof(mappings));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Public Methods

        public void Run() {
            foreach (var mapping in _mappings) {
                try {
                    var argumentType = GetArgumentType(mapping);
                    var classMapMethod = GetClassMapMethod(mapping, argumentType);
                    var bsonClassMap = CreateBsonClassMap(argumentType);
                    var classMapping = Activator.CreateInstance(mapping);

                    classMapMethod
                        .Invoke(
                            obj: classMapping,
                            parameters: [bsonClassMap]
                        );

                } catch (Exception ex) {
                    _logger.LogError(
                        exception: ex,
                        message: "Error while running mapping {mapping}",
                        args: mapping
                    );
                }
            }
        }

        #endregion

        #region Private Static

        private static Type GetArgumentType(Type? mappingType) {
            if (mappingType is null) {
                throw new InvalidOperationException("Argument type not found.");
            }

            var argumentType = mappingType.GetGenericArguments().FirstOrDefault();
            if (argumentType is not null) {
                return argumentType;
            }

            return GetArgumentType(mappingType.BaseType);
        }

        private static MethodInfo GetClassMapMethod(Type mappingType, Type argumentType) {
            var method = mappingType.GetMethod(
                    name: nameof(ClassMappingBase<object>.Map)
                )
                ?? throw new InvalidOperationException($"Method {nameof(ClassMappingBase<object>.Map)}<{argumentType.Name}>() not found.");

            return method;
        }

        private static object CreateBsonClassMap(Type argumentType) {
            var method = typeof(BsonClassMap)
                .GetMethods()
                .FirstOrDefault(m =>
                    m.IsGenericMethod &&
                    m.Name == nameof(BsonClassMap.RegisterClassMap)
                )
                ?? throw new InvalidOperationException($"Method {nameof(BsonClassMap.RegisterClassMap)}<{argumentType.Name}>() not found.");

            var result = method
                .MakeGenericMethod(
                    typeArguments: [argumentType]
                )
                .Invoke(
                    obj: null,
                    parameters: null
                );

            return result ?? throw new InvalidOperationException($"Impossible to execute {nameof(BsonClassMap)}.{nameof(BsonClassMap.RegisterClassMap)}<{argumentType.Name}>()");
        }

        #endregion
    }
}
