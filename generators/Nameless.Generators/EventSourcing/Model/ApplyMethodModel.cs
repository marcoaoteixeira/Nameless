using Nameless.Generators.Shared.Models;

namespace Nameless.Generators.EventSourcing.Model;

/// <summary>
///     All extracted data for a single method decorated with <c>[Apply]</c>.
/// </summary>
public sealed record ApplyMethodModel(
    ClassModel Class,
    string EventTypeFullName,
    string MethodName,
    LocationModel Location
);
