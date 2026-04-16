using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nameless.Helpers;

/// <summary>
///     Helper class for working with generic types.
/// </summary>
/// <remarks>
///     The "GenericTypeHelper" provides mechanisms to infer which types can be
///     used in an open generic type, given that the open generic imposes
///     certain constraints. For example, imagine you have a type
///     MyGenericList&lt;T&gt;, where T must be any type that implements the
///     IEntry interface. In this case, the helper attempts to locate all types
///     that implement IEntry within the provided assemblies and returns a list
///     of those types.
///
///     As a side note, the helper does not process generic types used as
///     constraints. That becomes a deep rabbit hole, and if you have a
///     constraint that itself is a generic type, it may be worth revisiting
///     the design.Therefore, constraints such as <c>where T : IGenericConstraint&lt;X&gt;</c>
///     will not be processed during type discovery. However, the helper will
///     still attempt to find all types that implement <c>IGenericConstraint&lt;X&gt;</c>
///     within the provided assemblies.
/// </remarks>
public static class GenericTypeHelper
{
    /// <summary>
    ///     Finds all types that can close a given generic definition type.
    /// </summary>
    /// <param name="genericDefinition">
    ///     The open generic type.
    /// </param>
    /// <param name="assemblies">
    ///     A list of assemblies to search in. If empty, uses default
    ///     assemblies.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}"/> of <see cref="Type"/> arrays,
    ///     where each array contains types that can close the open generic
    ///     type.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="genericDefinition"/> is not a generic type
    ///     definition.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if a generic parameter has no discoverable constraints,
    ///     making it impossible to find closing types.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="genericDefinition"/> or
    ///     <paramref name="assemblies"/> is <see langword="null"/>.
    /// </exception>
    public static IEnumerable<Type[]> GetArgumentsThatCloses(Type genericDefinition, params Assembly[] assemblies)
    {
        if (!genericDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException("The type must be a generic type definition.", nameof(genericDefinition));
        }

        var genericArgs = genericDefinition.GetGenericArguments();
        var searchAssemblies = assemblies.Length == 0
            ? GetDefaultAssemblies(genericDefinition)
            : assemblies;

        // Discover available types for each generic parameter
        var availableTypesPerParameter = new Type[genericArgs.Length][];
        for (var idx = 0; idx < genericArgs.Length; idx++)
        {
            availableTypesPerParameter[idx] = DiscoverTypesForParameter(genericArgs[idx], searchAssemblies).ToArray();
            if (availableTypesPerParameter[idx].Length != 0)
            {
                continue;
            }

            var errorMessage = $"""
                                Generic parameter '{genericArgs[idx].Name}' at position {idx} has no discoverable constraints.
                                Unable to determine available types for closing the generic type '{genericDefinition.Name}'.
                                Constraints found: {GetConstraintDescription(genericArgs[idx])}
                                """;

            throw new InvalidOperationException(errorMessage);
        }

        return FindValidCombinations(genericArgs, availableTypesPerParameter, genericDefinition);
    }

    /// <summary>
    /// Gets default assemblies to search in.
    /// </summary>
    private static Assembly[] GetDefaultAssemblies(Type genericDefinition)
    {
        var assemblies = new HashSet<Assembly>
        {
            genericDefinition.Assembly, // Assembly containing the open generic type
            Assembly.GetExecutingAssembly(), // Current assembly
            Assembly.GetCallingAssembly() // Calling assembly
        };

        // Add assemblies of constraint types
        var genericArgs = genericDefinition.GetGenericArguments();
        foreach (var arg in genericArgs)
        {
            var constraints = arg.GetGenericParameterConstraints();
            foreach (var constraint in constraints)
            {
                if (!constraint.IsGenericParameter)
                {
                    assemblies.Add(constraint.Assembly);
                }
            }
        }

        return assemblies.ToArray();
    }

    /// <summary>
    /// Discovers types that can satisfy the constraints of a generic parameter.
    /// </summary>
    private static HashSet<Type> DiscoverTypesForParameter(Type genericArg, Assembly[] assemblies)
    {
        var discoveredTypes = new HashSet<Type>();
        var attrs = genericArg.GenericParameterAttributes;
        var typeConstraints = genericArg.GetGenericParameterConstraints();

        // If no constraints at all, we can't discover types
        if (attrs == GenericParameterAttributes.None && typeConstraints.Length == 0)
        {
            return discoveredTypes;
        }

        // Get all types from the specified assemblies
        var allTypes = GetAllTypesFromAssemblies(assemblies);

        foreach (var type in allTypes)
        {
            try {
                if (SatisfiesParameterConstraints(genericArg, type))
                {
                    discoveredTypes.Add(type);
                }
            }
            catch { /* skip types that cause issues during constraint checking */ }
        }

        // If we have type constraints, also include the constraint
        // types themselves (if concrete)
        foreach (var constraint in typeConstraints)
        {
            if (constraint is not
                {
                    IsGenericParameter: false,
                    IsAbstract: false,
                    IsInterface: false
                })
            {
                continue;
            }

            if (SatisfiesParameterConstraints(genericArg, constraint))
            {
                discoveredTypes.Add(constraint);
            }
        }

        return discoveredTypes;
    }

    /// <summary>
    /// Gets all types from the specified assemblies, filtering out problematic types.
    /// </summary>
    private static Type[] GetAllTypesFromAssemblies(Assembly[] assemblies)
    {
        var allTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetExportedTypes()
                                    .Where(type => type is
                                    {
                                        // Exclude open generic types
                                        IsGenericTypeDefinition: false,

                                        // Exclude pointer types
                                        IsPointer: false,

                                        // Exclude by-ref types
                                        IsByRef: false,

                                        // Exclude by-ref-like types
                                        IsByRefLike: false,

                                        // Exclude abstract types (can't be instantiated)
                                        IsAbstract: false,

                                        // Only public types
                                        IsPublic: true
                                    } && !typeof(void).IsAssignableFrom(type));

                allTypes.AddRange(types);
            }
            catch { /* Ignore assembly if we can't get the exported types. */  }
        }

        return [.. allTypes];
    }

    /// <summary>
    /// Checks if a type satisfies all constraints of a generic parameter.
    /// </summary>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery", Justification = "Avoid convert to a LINQ operation when it makes difficult to read.")]
    private static bool SatisfiesParameterConstraints(Type genericArg, Type candidateType)
    {
        var attrs = genericArg.GenericParameterAttributes;

        // Check reference type constraint
        if ((attrs & GenericParameterAttributes.ReferenceTypeConstraint) != 0 && candidateType.IsValueType)
        {
            return false;
        }

        // Check value type constraint
        if ((attrs & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0 && (!candidateType.IsValueType || candidateType.IsNullable))
        {
            return false;
        }

        // Check new() constraint
        if ((attrs & GenericParameterAttributes.DefaultConstructorConstraint) != 0 && !candidateType.HasParameterlessConstructor())
        {
            return false;
        }

        // Check type constraints
        var typeConstraints = genericArg.GetGenericParameterConstraints();
        foreach (var constraint in typeConstraints)
        {
            if (!CanSatisfyTypeConstraint(constraint, candidateType))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a candidate type can satisfy a type constraint.
    /// </summary>
    private static bool CanSatisfyTypeConstraint(Type constraint, Type candidateType)
    {
        // Handle generic parameter constraints (cross-parameter references)
        if (constraint.IsGenericParameter)
        {
            // For cross-parameter constraints, we'll validate this during combination checking
            return true;
        }

        switch (constraint)
        {
            // Handle regular type constraints
            case { IsGenericType: false, ContainsGenericParameters: false }:
                return constraint.IsAssignableFrom(candidateType);
            // Handle open generic type constraints
            case { IsGenericType: true, ContainsGenericParameters: true }:
                return constraint.IsAssignableFromGeneric(candidateType);
        }

        // Handle generic constraints
        if (!candidateType.IsGenericType)
        {
            return false;
        }

        var constraintDefinition = constraint.GetGenericTypeDefinition();
        var candidateDefinition = candidateType.IsGenericTypeDefinition
            ? candidateType
            : candidateType.GetGenericTypeDefinition();

        return constraintDefinition.IsAssignableFrom(candidateDefinition) ||
               candidateType.GetInterfaces()
                            .Any(type => type.IsGenericType &&
                                         type.GetGenericTypeDefinition() == constraintDefinition);

    }

    /// <summary>
    /// Gets a detailed description of constraints for error reporting.
    /// </summary>
    private static string GetConstraintDescription(Type genericArg)
    {
        var constraints = new List<string>();
        var attrs = genericArg.GenericParameterAttributes;

        if ((attrs & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
        {
            constraints.Add("class");
        }

        if ((attrs & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
        {
            constraints.Add("struct");
        }

        if ((attrs & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
        {
            constraints.Add("new()");
        }

        var typeConstraints = genericArg.GetGenericParameterConstraints();
        constraints.AddRange(typeConstraints.Select(type => type.Name));

        return constraints.Count > 0 ? string.Join(", ", constraints) : "none";
    }

    /// <summary>
    /// Finds all valid combinations of types that can close the generic type.
    /// </summary>
    private static IEnumerable<Type[]> FindValidCombinations(Type[] genericArgs, Type[][] availableTypesPerParameter, Type openGeneric)
    {
        return GenerateCombinations(
                availableTypesPerParameter,
                parameterIndex: 0,
                new Type[genericArgs.Length]
            ).Where(combination => ValidateTypeArguments(openGeneric, combination));
    }

    /// <summary>
    /// Generates all possible combinations of types from the available types per parameter.
    /// </summary>
    private static IEnumerable<Type[]> GenerateCombinations(Type[][] availableTypesPerParameter, int parameterIndex, Type[] currentCombination)
    {
        if (parameterIndex >= availableTypesPerParameter.Length)
        {
            yield return (Type[])currentCombination.Clone();
            yield break;
        }

        foreach (var type in availableTypesPerParameter[parameterIndex])
        {
            currentCombination[parameterIndex] = type;

            var combinations = GenerateCombinations(
                availableTypesPerParameter,
                parameterIndex + 1,
                currentCombination
            );

            foreach (var combination in combinations)
            {
                yield return combination;
            }
        }
    }

    /// <summary>
    /// Validates that the provided type arguments satisfy all constraints including cross-parameter constraints.
    /// </summary>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery", Justification = "Avoid convert to a LINQ operation when it makes difficult to read.")]
    private static bool ValidateTypeArguments(Type openGeneric, Type[] typeArguments)
    {
        var genericParameters = openGeneric.GetGenericArguments();

        if (genericParameters.Length != typeArguments.Length)
        {
            return false;
        }

        for (var idx = 0; idx < genericParameters.Length; idx++)
        {
            if (!SatisfiesAllConstraints(genericParameters[idx], typeArguments[idx], typeArguments, genericParameters))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a type satisfies all constraints including cross-parameter constraints.
    /// </summary>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery", Justification = "Avoid convert to a LINQ operation when it makes difficult to read.")]
    private static bool SatisfiesAllConstraints(Type genericArg, Type candidateType, Type[] allTypeArguments, Type[] allGenericParameters)
    {
        var attrs = genericArg.GenericParameterAttributes;

        // Check basic constraints
        if ((attrs & GenericParameterAttributes.ReferenceTypeConstraint) != 0 && candidateType.IsValueType)
        {
            return false;
        }

        if ((attrs & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0 && (!candidateType.IsValueType || candidateType.IsNullable))
        {
            return false;
        }

        if ((attrs & GenericParameterAttributes.DefaultConstructorConstraint) != 0 && !candidateType.HasParameterlessConstructor())
        {
            return false;
        }

        // Check type constraints with cross-parameter resolution
        var typeConstraints = genericArg.GetGenericParameterConstraints();
        foreach (var constraint in typeConstraints)
        {
            if (!SatisfiesTypeConstraintWithResolution(constraint, candidateType, allTypeArguments, allGenericParameters))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks type constraints with cross-parameter resolution.
    /// </summary>
    private static bool SatisfiesTypeConstraintWithResolution(Type constraint, Type candidateType, Type[] typeArguments, Type[] genericParameters)
    {
        // Resolve cross-parameter constraints
        var resolvedConstraint = ResolveGenericConstraint(constraint, typeArguments, genericParameters);

        return resolvedConstraint is not null && resolvedConstraint.IsAssignableFrom(candidateType);
    }

    /// <summary>
    /// Resolves a generic constraint by substituting type arguments for generic parameters.
    /// </summary>
    private static Type? ResolveGenericConstraint(Type constraint, Type[] typeArguments, Type[] genericParameters)
    {
        if (!constraint.ContainsGenericParameters)
        {
            return constraint;
        }

        try
        {
            if (constraint.IsGenericParameter)
            {
                var paramIndex = Array.IndexOf(genericParameters, constraint);

                return paramIndex >= 0 && paramIndex < typeArguments.Length
                    ? typeArguments[paramIndex]
                    : null;
            }

            if (constraint.IsGenericType)
            {
                var constraintArgs = constraint.GetGenericArguments();
                var resolvedArgs = new List<Type>();

                foreach (var constraintArg in constraintArgs)
                {
                    var resolvedArg = ResolveGenericConstraint(constraintArg, typeArguments, genericParameters);
                    if (resolvedArg is null)
                    {
                        return null;
                    }

                    resolvedArgs.Add(resolvedArg);
                }

                return constraint.GetGenericTypeDefinition().MakeGenericType(resolvedArgs.ToArray());
            }
        }
        catch { return null; }

        return constraint;
    }
}
