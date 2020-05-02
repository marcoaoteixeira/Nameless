using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.IoC;

namespace Nameless.CQRS {
    public sealed class Dispatcher : IDispatcher {
        #region Private Read-Only Fields

        private readonly IServiceResolver _serviceResolver;

        #endregion

        #region Public Constructors

        public Dispatcher (IServiceResolver serviceResolver) {
            Prevent.ParameterNull (serviceResolver, nameof (serviceResolver));

            _serviceResolver = serviceResolver;
        }

        #endregion

        #region IDispatcher Members

        public Task CommandAsync<TCommand> (TCommand command, IProgress<int> progress = null, CancellationToken token = default) where TCommand : ICommand {
            Prevent.ParameterNull (command, nameof (command));

            var handlerType = typeof (ICommandHandler<>).MakeGenericType (command.GetType ());
            dynamic handler = _serviceResolver.Get (handlerType);

            return handler.HandleAsync ((dynamic)command, progress ?? NullProgress.Instance, token);
        }

        public Task<TResult> QueryAsync<TResult> (IQuery<TResult> query, IProgress<int> progress = null, CancellationToken token = default) {
            Prevent.ParameterNull (query, nameof (query));

            var handlerType = typeof (IQueryHandler<,>).MakeGenericType (query.GetType (), typeof (TResult));
            dynamic handler = _serviceResolver.Get (handlerType);

            return (Task<TResult>)handler.HandleAsync ((dynamic)query, progress ?? NullProgress.Instance, token);
        }

        #endregion
    }
}
