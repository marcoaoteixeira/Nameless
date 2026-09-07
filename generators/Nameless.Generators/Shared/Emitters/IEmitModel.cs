using Nameless.Generators.Shared.Models;

namespace Nameless.Generators.Shared.Emitters;

public interface IEmitModel {
    ClassModel Class { get; }
}