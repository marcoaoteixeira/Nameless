// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0180:Use tuple to swap values", Justification = "Change will make it to hard to read.", Scope = "member", Target = "~M:Nameless.Security.RandomPasswordGenerator.Generate(Nameless.Security.PasswordOptions)~System.String")]
[assembly: SuppressMessage("Maintainability", "CA1513:Use ObjectDisposedException throw helper", Justification = "ThrowIf is not available for netstandard2.1, to keep code consistent we'll use the common pattern.", Scope = "member", Target = "~M:Nameless.Security.Crypto.RijndaelCryptographicService.BlockAccesAfterDispose")]
[assembly: SuppressMessage("Style", "IDE0078:Use pattern matching", Justification = "Use pattern matching here will just make the core harder to read.", Scope = "member", Target = "~P:Nameless.Security.Options.RijndaelCryptoOptions.MinimumSaltSize")]
[assembly: SuppressMessage("Style", "IDE0078:Use pattern matching", Justification = "Use pattern matching here will just make the core harder to read.", Scope = "member", Target = "~P:Nameless.Security.Options.RijndaelCryptoOptions.MaximumSaltSize")]
