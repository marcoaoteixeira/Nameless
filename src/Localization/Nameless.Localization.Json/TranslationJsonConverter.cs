using System.Text.Json;
using System.Text.Json.Serialization;
using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {
    public sealed class TranslationJsonConverter : JsonConverter<Translation> {
        #region Public Static Read-Only Properties

        public static TranslationJsonConverter Default => new();

        #endregion

        #region Public Override Methods

        public override bool CanConvert(Type typeToConvert)
            => typeof(Translation) == typeToConvert;

        public override Translation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var json = JsonElement.ParseValue(ref reader);
            var culture = json.GetProperty(nameof(Translation.Culture));
            var containers = json.GetProperty("Containers");

            var result = new Translation(new(culture.GetString() ?? string.Empty));

            // Undefined culture.
            if (string.IsNullOrWhiteSpace(result.Culture.Name)) {
                return result;
            }

            foreach (var containerItem in containers.EnumerateObject()) {
                var container = new Container(containerItem.Name);

                foreach (var entryItem in containerItem.Value.EnumerateObject()) {
                    var entry = new Entry(entryItem.Name, entryItem.Value.GetString() ?? string.Empty);

                    container.Add(entry);
                }

                result.Add(container);
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Translation value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
