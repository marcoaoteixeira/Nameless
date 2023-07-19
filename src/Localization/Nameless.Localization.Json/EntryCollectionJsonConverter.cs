using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nameless.Localization.Json.Schema;
using Nameless.Serialization;

namespace Nameless.Localization.Json {

    public sealed class EntryCollectionJsonConverter : JsonConverter<EntryCollection> {

        #region Public Static Read-Only Properties

        public static EntryCollectionJsonConverter Default => new();

        #endregion

        #region Public Override Methods

        public override bool CanConvert(Type typeToConvert)
            => typeof(EntryCollection) == typeToConvert
            || typeof(EntryCollection[]) == typeToConvert
            || typeof(IList<EntryCollection>) == typeToConvert
            || typeof(List<EntryCollection>) == typeToConvert
            || typeof(HashSet<EntryCollection>) == typeToConvert
            || typeof(Collection<EntryCollection>) == typeToConvert;

        public override EntryCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            EntryCollection entryCollection;
            Entry entry;

            var json = JsonElement.ParseValue(ref reader);
            foreach (var jsonEntryCollection in json.EnumerateObject()) {
                entryCollection = new(jsonEntryCollection.Name);

                foreach (var jsonEntry in jsonEntryCollection.Value.EnumerateObject()) {
                    entry = new(jsonEntry.Name);

                    foreach (var jsonValue in jsonEntry.Value.EnumerateArray()) {
                        entry.Add(jsonValue.GetString() ?? string.Empty);
                    }
                }
            }




            if (reader.TokenType == JsonTokenType.Null) { return EntryCollection.Empty; }
            if (reader.TokenType == JsonTokenType.String) { return options. serializer.Deserialize(reader, objectType); }

            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, EntryCollection value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Static Methods

        private static object Convert(ISet<EntryCollection> messageCollectionSet, Type resultType) {
            if (resultType == typeof(EntryCollection[])) {
                return messageCollectionSet.ToArray();
            }

            if (resultType == typeof(IList<EntryCollection>) || resultType == typeof(List<EntryCollection>)) {
                return messageCollectionSet.ToList();
            }

            if (resultType == typeof(HashSet<EntryCollection>)) {
                return messageCollectionSet;
            }

            return new Collection<EntryCollection>(messageCollectionSet.ToList());
        }

        #endregion
    }
}
