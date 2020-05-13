using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.PubSub {
    public class Publisher : PubSubBase, IPublisher {
        #region IPublisher Members

        public Task PublishAsync (string topic, Message message, CancellationToken token = default) {
            Prevent.ParameterNull (message, nameof (message));
            Prevent.ParameterNullOrWhiteSpace (topic, nameof (topic));

            if (!Cache.ContainsKey (topic)) { return Task.CompletedTask; }

            Subscription[] subscriptions;
            lock (SyncLock) { subscriptions = Cache[topic].ToArray (); }
            foreach (var subscription in subscriptions) {
                token.ThrowIfCancellationRequested ();
                Action<Message> handler = null;
                try { handler = subscription.CreateHandler (); }
                catch (ObjectDisposedException) { /* Just ignore */ }
                catch (Exception) { throw; }
                if (handler == null) { continue; }
                handler (message);
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}