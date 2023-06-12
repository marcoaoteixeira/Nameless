using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    internal static class BasicPropertiesExtension {

        #region Internal Static Methods

        internal static IBasicProperties FillWith(this IBasicProperties self, Arguments arguments) {
            self.AppId = arguments.Get<string?>(ArgumentKeys.BP_APPID);
            self.ClusterId = arguments.Get<string?>(ArgumentKeys.BP_CLUSTERID);
            self.ContentEncoding = arguments.Get<string?>(ArgumentKeys.BP_CONTENTENCODING);
            self.ContentType = arguments.Get<string?>(ArgumentKeys.BP_CONTENTTYPE);
            self.CorrelationId = arguments.Get<string?>(ArgumentKeys.BP_CORRELATIONID);
            self.DeliveryMode = arguments.Get<byte>(ArgumentKeys.BP_DELIVERYMODE);
            self.Expiration = arguments.Get<string?>(ArgumentKeys.BP_EXPIRATION);
            self.Headers = arguments.Get<IDictionary<string, object>>(ArgumentKeys.BP_HEADERS);
            self.MessageId = arguments.Get<string?>(ArgumentKeys.BP_MESSAGEID);
            self.Persistent = arguments.Get<bool>(ArgumentKeys.BP_PERSISTENT);
            self.Priority = arguments.Get<byte>(ArgumentKeys.BP_PRIORITY);
            self.ReplyTo = arguments.Get<string?>(ArgumentKeys.BP_REPLYTO);

            var replyToAddress = arguments.Get<PublicationAddress?>(ArgumentKeys.BP_REPLYTOADDRESS);
            if (replyToAddress != default) { self.ReplyToAddress = replyToAddress; }

            self.Timestamp = arguments.Get<AmqpTimestamp>(ArgumentKeys.BP_TIMESTAMP);
            self.Type = arguments.Get<string?>(ArgumentKeys.BP_TYPE);
            self.UserId = arguments.Get<string?>(ArgumentKeys.BP_USERID);

            return self;
        }

        #endregion
    }
}
