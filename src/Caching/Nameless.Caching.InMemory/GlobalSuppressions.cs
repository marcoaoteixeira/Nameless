// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1513:Use ObjectDisposedException throw helper", Justification = "ThrowIf is not available for netstandard2.1, to keep code consistent we'll use the common pattern.", Scope = "member", Target = "~M:Nameless.Caching.InMemory.InMemoryCache.BlockAccessAfterDispose")]
