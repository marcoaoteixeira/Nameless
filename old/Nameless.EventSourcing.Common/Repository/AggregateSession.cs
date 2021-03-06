using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nameless.EventSourcing.Domain;

namespace Nameless.EventSourcing.Repository {
    public sealed class AggregateSession : ISession {
        #region Private Read-Only Fields

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim (initialCount: 1, maxCount: 1);
        private readonly IDictionary<string, object> _cache = new Dictionary<string, object> ();
        private readonly IRepository _repository;

        #endregion

        #region Public Constructors

        public AggregateSession (IRepository repository) {
            Prevent.ParameterNull (repository, nameof (repository));

            _repository = repository;
        }

        #endregion

        #region Private Static Methods

        private static string GetCacheKeyPrefix<TAggregate> () where TAggregate : AggregateRoot {
            return $"[{typeof (TAggregate).FullName}]";
        }

        private static string GetCacheKeyFor<TAggregate> (Guid aggregateID) where TAggregate : AggregateRoot {
            var prefix = GetCacheKeyPrefix<TAggregate> ();
            return $"{prefix} : {aggregateID}";
        }

        #endregion

        #region Private Methods

        private IEnumerable<string> GetCurrentCacheKeys<TAggregate> () where TAggregate : AggregateRoot {
            var prefix = GetCacheKeyPrefix<TAggregate> ();
            return _cache.Keys.Where (_ => _.StartsWith (prefix)).ToArray ();
        }

        #endregion

        #region ISession Members

        public void Attach<TAggregate> (TAggregate aggregate) where TAggregate : AggregateRoot {
            Prevent.ParameterNull (aggregate, nameof (aggregate));

            _semaphore.Wait ();
            try {
                var key = GetCacheKeyFor<TAggregate> (aggregate.AggregateID);
                if (!_cache.ContainsKey (key)) {
                    _cache.Add (key, aggregate);
                }
            } finally { _semaphore.Release (); }
        }

        public void Detach<TAggregate> (TAggregate aggregate) where TAggregate : AggregateRoot {
            Prevent.ParameterNull (aggregate, nameof (aggregate));

            _semaphore.Wait ();
            try {
                var key = GetCacheKeyFor<TAggregate> (aggregate.AggregateID);
                if (_cache.ContainsKey (key)) {
                    _cache.Remove (key);
                }
            } finally { _semaphore.Release (); }
        }

        public void DetachAll<TAggregate> () where TAggregate : AggregateRoot {
            _semaphore.Wait ();
            try {
                var keys = GetCurrentCacheKeys<TAggregate> ();
                foreach (var key in keys) {
                    _cache.Remove (key);
                }
            } finally { _semaphore.Release (); }
        }

        public Task<TAggregate> GetAsync<TAggregate> (Guid aggregateID, CancellationToken token = default) where TAggregate : AggregateRoot {
            _semaphore.Wait ();
            TAggregate result = default;
            try {
                var key = GetCacheKeyFor<TAggregate> (aggregateID);
                if (_cache.TryGetValue (key, out object aggregate)) {
                    result = (TAggregate) aggregate;
                }
            } finally { _semaphore.Release (); }
            return Task.FromResult (result);
        }

        public Task CommittAsync<TAggregate> (CancellationToken token = default) where TAggregate : AggregateRoot {
            _semaphore.Wait ();
            try {
                var keys = GetCurrentCacheKeys<TAggregate> ();
                foreach (var key in keys) {
                    token.ThrowIfCancellationRequested ();
                    if (_cache.TryGetValue (key, out object aggregate)) {
                        _repository.SaveAsync ((TAggregate) aggregate, token);
                    }
                }
            } finally { _semaphore.Release (); }
            return Task.CompletedTask;
        }

        #endregion
    }
}