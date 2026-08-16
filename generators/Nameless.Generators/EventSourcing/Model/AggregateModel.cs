using System.Collections.Immutable;
using Nameless.Generators.Emitters;
using Nameless.Generators.Models;

namespace Nameless.Generators.EventSourcing.Model;

/// <summary>
///     Every <c>[Apply]</c> method declared on a single aggregate class,
///     ready to be emitted as its <c>When</c> dispatch override.
/// </summary>
public sealed record AggregateModel(ClassModel Class, ImmutableArray<ApplyMethodModel> ApplyMethods) : IEmitModel;
