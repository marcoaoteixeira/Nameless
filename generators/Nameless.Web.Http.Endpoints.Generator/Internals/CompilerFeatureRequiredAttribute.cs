// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices;

/// <summary>
///     Indicates that compiler support for a particular feature is
///     required for the location where this attribute is applied.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute {
    /// <summary>
    ///     The name of the compiler feature.
    /// </summary>
    internal string FeatureName { get; }

    /// <summary>
    ///     If true, the compiler can choose to allow access to the location
    ///     where this attribute is applied if it does not understand
    ///     <see cref="FeatureName"/>.
    /// </summary>
    internal bool IsOptional { get; init; }

    /// <summary>
    ///     The <see cref="FeatureName"/> used for the ref structs
    ///     C# feature.
    /// </summary>
    internal const string RefStructs = nameof(RefStructs);

    /// <summary>
    ///     The <see cref="FeatureName"/> used for the required
    ///     members C# feature.
    /// </summary>
    internal const string RequiredMembers = nameof(RequiredMembers);

    internal CompilerFeatureRequiredAttribute(string featureName) {
        FeatureName = featureName;
    }
}