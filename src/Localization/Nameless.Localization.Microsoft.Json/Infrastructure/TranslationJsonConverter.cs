using System.Text.Json;
using System.Text.Json.Serialization;
using Nameless.Localization.Microsoft.Json.Objects;

namespace Nameless.Localization.Microsoft.Json.Infrastructure {
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
            var regions = json.GetProperty(nameof(Translation.Regions));

            var translation = new Translation(new(culture.GetString() ?? string.Empty));

            // Undefined culture.
            if (string.IsNullOrWhiteSpace(translation.Culture.Name)) {
                return translation;
            }

            foreach (var regionObject in regions.EnumerateObject()) {
                var region = new Region(regionObject.Name);

                foreach (var messageObject in regionObject.Value.EnumerateObject()) {
                    var message = new Message(messageObject.Name, messageObject.Value.GetString() ?? string.Empty);

                    region.Add(message);
                }

                translation.Add(region);
            }

            return translation;
        }

        public override void Write(Utf8JsonWriter writer, Translation value, JsonSerializerOptions options)
            => throw new NotImplementedException();

        #endregion
    }
}
