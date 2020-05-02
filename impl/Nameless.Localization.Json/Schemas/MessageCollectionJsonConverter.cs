using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nameless.Localization.Json.Schemas {
    public sealed class MessageCollectionJsonConverter : JsonConverter {
        #region Public Override Methods

        public override bool CanConvert (Type objectType) {
            return typeof (MessageCollection[]) == objectType ||
                typeof (IList<MessageCollection>) == objectType ||
                typeof (List<MessageCollection>) == objectType ||
                typeof (HashSet<MessageCollection>) == objectType ||
                typeof (Collection<MessageCollection>) == objectType;
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) { return string.Empty; }
            if (reader.TokenType == JsonToken.String) { return serializer.Deserialize (reader, objectType); }

            var messageCollectionSet = new HashSet<MessageCollection> ();
            var messageCollections = JObject.Load (reader).Values<JProperty> ().ToArray ();
            foreach (var messageCollection in messageCollections) {
                var messageSet = new HashSet<Message> ();
                var messages = messageCollection.Values<JObject> ().Values<JProperty> ().ToArray ();
                foreach (var message in messages) {
                    var translations = message.Values<JToken> ().Values<string> ().ToArray ();
                    messageSet.Add (new Message (message.Name, translations));
                }
                messageCollectionSet.Add (new MessageCollection (messageCollection.Name, messageSet.ToArray ()));
            }
            return Convert (messageCollectionSet, objectType);
        }

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException ();
        }

        #endregion

        #region Private Static Methods

        private static object Convert (ISet<MessageCollection> messageCollectionSet, Type resultType) {
            if (resultType == typeof (MessageCollection[])) {
                return messageCollectionSet.ToArray ();
            }

            if (resultType == typeof (IList<MessageCollection>) || resultType == typeof (List<MessageCollection>)) {
                return messageCollectionSet.ToList ();
            }

            if (resultType == typeof (HashSet<MessageCollection>)) {
                return messageCollectionSet;
            }

            return new Collection<MessageCollection> (messageCollectionSet.ToList ());
        }

        #endregion
    }
}