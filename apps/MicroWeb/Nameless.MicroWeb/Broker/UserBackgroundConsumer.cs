using Nameless.ProducerConsumer.AspNetCore;
using Nameless.ProducerConsumer;
using Nameless.MicroWeb.Models;

namespace Nameless.MicroWeb.Broker {
    public sealed class UserBackgroundConsumer : BackgroundConsumerBase<User> {
        #region Public Constructors

        public UserBackgroundConsumer(IConsumer consumer, Arguments? arguments = default)
            : base(consumer, "Nameless.MicroWeb.Queue", arguments) { }

        #endregion

        #region Protected Override Methods

        protected override void Consume(User payload) {
            Console.WriteLine($"User: Id = ${payload.Id}, Name = ${payload.Name}, Email = ${payload.Email}");
        }

        #endregion
    }
}
