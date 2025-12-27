using Nameless.ObjectModel;

namespace Nameless.Microservices.App.Infrastructure.Mediator;

public abstract record ResponseBase(Error[] Errors) {
    public bool Succeeded => Errors.Length == 0;
}
