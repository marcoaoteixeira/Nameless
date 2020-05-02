using System;
using System.Collections.Generic;

namespace Nameless.PubSub {
    public class Subscriber : PubSubBase, ISubscriber {

        #region ISubscriber Members

        public Subscription Subscribe (string topic, Action<Message> handler) {
            Prevent.ParameterNullOrWhiteSpace (topic, nameof (topic));
            Prevent.ParameterNull (handler, nameof (handler));

            var subscription = new Subscription (topic, handler);;
            lock (SyncLock) {
                // Create set if not exists
                if (!Cache.ContainsKey (topic)) {
                    Cache.Add (topic, new List<Subscription> ());
                }
                // Verify if the subscription is already in the cache,
                // if not, add, otherwise, get the current subscription
                // and return.
                var index = Cache[topic].IndexOf (subscription);
                if (index < 0) { Cache[topic].Add (subscription); } else { subscription = Cache[topic][index]; }
            }
            return subscription;
        }

        public bool Unsubscribe (Subscription subscription) {
            Prevent.ParameterNull (subscription, nameof (subscription));

            var result = false;
            lock (SyncLock) {
                if (Cache.ContainsKey (subscription.Topic)) {
                    if (Cache[subscription.Topic].Remove (subscription)) {
                        subscription.Dispose ();
                    }
                }
            }
            return result;
        }

        #endregion
    }
}