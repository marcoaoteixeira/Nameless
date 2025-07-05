﻿using System.Reflection;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public sealed record ValidationOptions {
    /// <summary>
    ///     Gets an array of assemblies to scan for validators.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];
}