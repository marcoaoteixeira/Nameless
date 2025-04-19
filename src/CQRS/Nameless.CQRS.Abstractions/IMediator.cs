using Nameless.CQRS.Events;
using Nameless.CQRS.Requests;
using Nameless.CQRS.Streams;

namespace Nameless.CQRS;

public interface IMediator : IRequester, IPublisher, IStreamer;