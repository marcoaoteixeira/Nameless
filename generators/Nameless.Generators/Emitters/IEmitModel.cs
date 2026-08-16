using Nameless.Generators.Models;

namespace Nameless.Generators.Emitters;

public interface IEmitModel {
    ClassModel Class { get; }
}