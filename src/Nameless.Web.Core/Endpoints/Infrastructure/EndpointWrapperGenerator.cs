using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

public interface IEndpointWrapperGenerator {
    Type Create(IEndpointDescriptor descriptor);

    void SetWrapperTarget(object wrapperInstance, IEndpoint endpoint);
}

public class EndpointWrapperGenerator : IEndpointWrapperGenerator {
    private const string TARGET_FIELD_NAME = "__target__";
    private const string SET_TARGET_METHOD_NAME = "__set_target__";

    private readonly ConcurrentDictionary<Type, Type> _cache = [];
    private readonly ModuleBuilder _moduleBuilder;

    public EndpointWrapperGenerator() {
        var assemblyName = new AssemblyName($"EndpointWrapperAssembly__{Guid.NewGuid():N}");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        _moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
    }

    public Type Create(IEndpointDescriptor descriptor) {
        Prevent.Argument.Null(descriptor);

        if (descriptor.EndpointType is null) {
            throw new InvalidOperationException($"Endpoint descriptor is missing '{nameof(IEndpointDescriptor.EndpointType)}' property.");
        }

        if (descriptor.EndpointType.IsAbstract || descriptor.EndpointType.IsInterface) {
            throw new InvalidOperationException("Endpoint type must be a concrete type.");
        }

        if (descriptor.Action is null) {
            throw new InvalidOperationException($"Endpoint descriptor is missing '{nameof(IEndpointDescriptor.Action)}' property.");
        }

        return _cache.GetOrAdd(descriptor.EndpointType, _ => InnerCreate(descriptor));
    }

    public void SetWrapperTarget(object wrapperInstance, IEndpoint endpoint) {
        Prevent.Argument.Null(wrapperInstance);
        Prevent.Argument.Null(endpoint);

        var handler = wrapperInstance.GetType().GetMethod(SET_TARGET_METHOD_NAME);
        if (handler is null) {
            throw new InvalidOperationException("Set wrapper target method not found.");
        }

        handler.Invoke(wrapperInstance, [endpoint]);
    }

    private Type InnerCreate(IEndpointDescriptor descriptor) {
        // creates the wrapper type name.
        // if the endpoint type name is 'HelloWorldEndpoint',
        // the wrapper type will be something like 'W__HelloWorldEndpoint__W'.
        var typeName = CreateTypeName(descriptor);

        // creates the type builder for our wrapper
        var typeBuilder = CreateTypeBuilder(typeName);

        // generates an empty constructor in our wrapper type so it can be
        // easily constructed without trouble.
        GenerateConstructor(typeBuilder);

        // creates a field inside the wrapper to hold
        // the instance for our actual target.
        var targetBuilder = CreateTargetBuilder(typeBuilder);

        // create the method builder and sets the necessary
        // infrastructure to call it.
        var methodBuilder = CreateMethodBuilder(typeBuilder, descriptor);

        // generates the forwarding call to effectively calling
        // the target instance method.
        GenerateForwardingCall(methodBuilder, targetBuilder, descriptor);

        return typeBuilder.CreateType();
    }

    private TypeBuilder CreateTypeBuilder(string typeName) {
        var typeBuilder = _moduleBuilder.DefineType(
            name: typeName,
            attr: TypeAttributes.Public | TypeAttributes.Class,
            parent: typeof(object)
        );

        return typeBuilder;
    }

    private static string CreateTypeName(IEndpointDescriptor descriptor) {
        var genericArgumentsCount = descriptor.EndpointType.GetGenericArguments().Length;
        var nameWithoutArity = descriptor.EndpointType.Name.Split('`')[0];
        var className = genericArgumentsCount > 0
            ? $"{nameWithoutArity}_T{genericArgumentsCount}"
            : nameWithoutArity;

        return $"W__{className}__W";
    }

    private static void GenerateConstructor(TypeBuilder typeBuilder) {
        var constructor = typeBuilder.DefineConstructor(
            attributes: MethodAttributes.Public,
            callingConvention: CallingConventions.Standard,
            parameterTypes: Type.EmptyTypes
        );

        var il = constructor.GetILGenerator();

        // Call base constructor
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(
            opcode: OpCodes.Call,
            con: typeof(object).GetConstructor(Type.EmptyTypes)
                 ?? throw new InvalidOperationException("Could not find suitable constructor for type 'object'.")
        );
        il.Emit(OpCodes.Ret);
    }

    private static FieldBuilder CreateTargetBuilder(TypeBuilder typeBuilder) {
        var targetField = typeBuilder.DefineField(
            fieldName: TARGET_FIELD_NAME,
            type: typeof(object),
            attributes: FieldAttributes.Private
        );

        var method = typeBuilder.DefineMethod(
            name: SET_TARGET_METHOD_NAME,
            attributes: MethodAttributes.Public | MethodAttributes.Virtual,
            returnType: typeof(void),
            parameterTypes: [typeof(object)]
        );

        var il = method.GetILGenerator();

        // this._target = target;
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, targetField);
        il.Emit(OpCodes.Ret);

        return targetField;
    }

    private static MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder, IEndpointDescriptor descriptor) {
        var action = descriptor.GetAction();
        var parameters = action.GetParameters();
        var parameterTypes = parameters.Select(parameter => parameter.ParameterType)
                                       .ToArray();

        // defines the wrapper method
        var methodBuilder = typeBuilder.DefineMethod(
            name: action.Name,
            attributes: MethodAttributes.Public | MethodAttributes.Virtual,
            returnType: action.ReturnType,
            parameterTypes: parameterTypes
        );

        // defines the method parameters, copy everything from source.
        foreach (var tuple in parameters.Index()) {
            var parameterBuilder = methodBuilder.DefineParameter(
                tuple.Index + 1,
                tuple.Item.Attributes,
                tuple.Item.Name
            );

            // copy custom attributes from the original method parameter
            // such as [FromQuery], [AsParameters] etc.
            foreach (var customAttribute in tuple.Item.CustomAttributes) {
                var customAttributeBuilder = CreateAttributeBuilder(customAttribute);

                if (customAttributeBuilder is null) {
                    continue;
                }

                parameterBuilder.SetCustomAttribute(customAttributeBuilder);
            }
        }

        return methodBuilder;
    }

    private static CustomAttributeBuilder? CreateAttributeBuilder(CustomAttributeData attrData) {
        try {
            var constructorArgs = attrData.ConstructorArguments
                                          .Select(arg => arg.Value)
                                          .ToArray();

            var namedProps = attrData.NamedArguments
                                     .Where(arg => arg.IsField == false)
                                     .Select(arg => (PropertyInfo)arg.MemberInfo)
                                     .ToArray();

            var propValues = attrData.NamedArguments
                                     .Where(arg => arg.IsField == false)
                                     .Select(arg => arg.TypedValue.Value)
                                     .ToArray();

            var namedFields = attrData.NamedArguments
                                      .Where(arg => arg.IsField)
                                      .Select(arg => (FieldInfo)arg.MemberInfo)
                                      .ToArray();

            var fieldValues = attrData.NamedArguments
                                      .Where(arg => arg.IsField)
                                      .Select(arg => arg.TypedValue.Value)
                                      .ToArray();

            return new CustomAttributeBuilder(
                attrData.Constructor,
                constructorArgs,
                namedProps,
                propValues,
                namedFields,
                fieldValues
            );
        }
        catch { return null; } // Not all attributes can be built dynamically (e.g., complex types)
    }

    private static void GenerateForwardingCall(MethodBuilder methodBuilder, FieldBuilder targetBuilder, IEndpointDescriptor descriptor) {
        var il = methodBuilder.GetILGenerator();

        // Check if target is null
        var targetNotNullLabel = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, targetBuilder);
        il.Emit(OpCodes.Brtrue_S, targetNotNullLabel);

        // Throw exception if target is null
        il.Emit(OpCodes.Ldstr, "Wrapper target has not been set.");
        il.Emit(
            opcode: OpCodes.Newobj,
            con: typeof(InvalidOperationException).GetConstructor([typeof(string)])
                 ?? throw new InvalidOperationException("Could not find suitable constructor for type 'InvalidOperationException'.")
        );
        il.Emit(OpCodes.Throw);

        il.MarkLabel(targetNotNullLabel);

        // Load target instance
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, targetBuilder);
        il.Emit(OpCodes.Castclass, descriptor.EndpointType);

        // Load all arguments
        var action = descriptor.GetAction();
        var parameterTypes = action.GetParameters()
                                   .Select(parameter => parameter.ParameterType)
                                   .ToArray();
        for (var index = 0; index < parameterTypes.Length; index++) {
            il.Emit(OpCodes.Ldarg, index + 1);
        }

        // Call the concrete method
        il.Emit(OpCodes.Callvirt, action);

        // Return
        il.Emit(OpCodes.Ret);
    }
}
