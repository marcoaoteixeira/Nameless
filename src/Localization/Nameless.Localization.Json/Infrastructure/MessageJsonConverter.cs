using System.Text.Json;
using System.Text.Json.Serialization;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json.Infrastructure;
public sealed class MessageJsonConverter : JsonConverter<Message> {
    public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartObject) {
            throw new JsonException("Invalid start object token.");
        }

        var id = string.Empty;
        var text = string.Empty;
        var done = false;

        while (!done && reader.Read()) {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (reader.TokenType) {
                case JsonTokenType.PropertyName:
                    id = reader.GetString() ?? string.Empty;
                    break;

                case JsonTokenType.String:
                    text = reader.GetString() ?? string.Empty;
                    break;

                case JsonTokenType.EndObject:
                    done = true;
                    break;
            }
        }

        return new Message(id, text);
    }

    public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName(value.Id);
        writer.WriteStringValue(value.Text);
        writer.WriteEndObject();
    }
}
