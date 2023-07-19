using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class ProducerArgsExtension {
        #region Private Constants

        private const string EXCHANGE_NAME_TOKEN = "ExchangeName";

        #endregion

        #region Public Static Methods

        public static string GetExchangeName(this ProducerArgs self) {
            var arg = self.Get(EXCHANGE_NAME_TOKEN) ?? Constants.DEFAULT_EXCHANGE_NAME;

            return (string)arg;
        }

        public static void SetExchangeName(this ProducerArgs self, string value) {
            self.Set(EXCHANGE_NAME_TOKEN, value ?? Constants.DEFAULT_EXCHANGE_NAME);
        }

        public static string? GetAppId(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.AppId));

            return (string?)arg;
        }

        public static void SetAppId(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.AppId), value);
            }
        }

        public static string? GetClusterId(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.ClearAppId));

            return (string?)arg;
        }

        public static void SetClusterId(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.ClusterId), value);
            }
        }

        public static string? GetContentEncoding(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.ContentEncoding));

            return (string?)arg;
        }

        public static void SetContentEncoding(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.ContentEncoding), value);
            }
        }

        public static string? GetContentType(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.ContentType));

            return (string?)arg;
        }

        public static void SetContentType(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.ContentType), value);
            }
        }

        public static string? GetCorrelationId(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.CorrelationId));

            return (string?)arg;
        }

        public static void SetCorrelationId(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.CorrelationId), value);
            }
        }

        public static byte GetDeliveryMode(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.DeliveryMode)) ?? byte.MinValue;

            return (byte)arg;
        }

        public static void SetDeliveryMode(this ProducerArgs self, byte value) {
            self.Set(nameof(IBasicProperties.DeliveryMode), value);
        }

        public static string? GetExpiration(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Expiration));

            return (string?)arg;
        }

        public static void SetExpiration(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.Expiration), value);
            }
        }

        public static IDictionary<string, object> GetHeaders(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Headers)) ?? new Dictionary<string, object>();

            return (IDictionary<string, object>)arg;
        }

        public static void SetHeaders(this ProducerArgs self, IDictionary<string, object> value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.Headers), value);
            }
        }

        public static string? GetMessageId(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.MessageId));

            return (string?)arg;
        }

        public static void SetMessageId(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.MessageId), value);
            }
        }

        public static bool GetPersistent(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Persistent)) ?? false;

            return (bool)arg;
        }

        public static void SetPersistent(this ProducerArgs self, bool value) {
            self.Set(nameof(IBasicProperties.Persistent), value);
        }

        public static byte GetPriority(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Priority)) ?? byte.MinValue;

            return (byte)arg;
        }

        public static void SetPriority(this ProducerArgs self, byte value) {
            self.Set(nameof(IBasicProperties.Priority), value);
        }

        public static string? GetReplyTo(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.ReplyTo));

            return (string?)arg;
        }

        public static void SetReplyTo(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.ReplyTo), value);
            }
        }

        public static PublicationAddress? GetReplyToAddress(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.ReplyToAddress));

            return (PublicationAddress?)arg;
        }

        public static void SetReplyToAddress(this ProducerArgs self, PublicationAddress? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.ReplyToAddress), value);
            }
        }

        public static AmqpTimestamp GetTimestamp(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Timestamp)) ?? default(AmqpTimestamp);

            return (AmqpTimestamp)arg;
        }

        public static void SetTimestamp(this ProducerArgs self, AmqpTimestamp value) {
            self.Set(nameof(IBasicProperties.Timestamp), value);
        }

        public static string? GetTypeProp(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.Type));

            return (string?)arg;
        }

        public static void SetTypeProp(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.Type), value);
            }
        }

        public static string? GetUserId(this ProducerArgs self) {
            var arg = self.Get(nameof(IBasicProperties.UserId));

            return (string?)arg;
        }

        public static void SetUserId(this ProducerArgs self, string? value) {
            if (value != null) {
                self.Set(nameof(IBasicProperties.UserId), value);
            }
        }

        #endregion
    }
}
