using Nameless.Patterns.Mediator.Events;
using Nameless.Patterns.Mediator.Requests;
using Nameless.Patterns.Mediator.Streams;

namespace Nameless.Patterns.Mediator;

/// <summary>
/// The mediator interface.
/// </summary>
public interface IMediator : IRequestHandlerInvoker,
                             IEventHandlerInvoker,
                             IStreamHandlerInvoker;