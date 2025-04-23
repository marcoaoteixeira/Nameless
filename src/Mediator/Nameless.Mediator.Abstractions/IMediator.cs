using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
/// The mediator interface.
/// </summary>
public interface IMediator : IRequestHandlerProxy,
                             IEventHandlerProxy,
                             IStreamHandlerProxy;