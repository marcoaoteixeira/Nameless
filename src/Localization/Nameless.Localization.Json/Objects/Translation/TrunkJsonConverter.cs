using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nameless.Localization.Json.Objects.Translation {
    public sealed class TrunkJsonConverter : JsonConverter<Trunk> {
        #region Public Static Read-Only Properties

        public static TrunkJsonConverter Default => new();

        #endregion

        #region Public Override Methods

        public override bool CanConvert(Type typeToConvert)
            => typeof(Trunk) == typeToConvert;

        public override Trunk? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var json = JsonElement.ParseValue(ref reader);
            var culture = json.GetProperty(nameof(Trunk.Culture));
            var branches = json.GetProperty(nameof(Trunk.Branches));

            var trunk = new Trunk(new(culture.GetString() ?? string.Empty));

            // Undefined culture.
            if (string.IsNullOrWhiteSpace(trunk.Culture.Name)) {
                return trunk;
            }

            foreach (var branchObject in branches.EnumerateObject()) {
                var branch = new Branch(branchObject.Name);

                foreach (var leafObject in branchObject.Value.EnumerateObject()) {
                    var leaf = new Leaf(leafObject.Name, leafObject.Value.GetString() ?? string.Empty);

                    branch.Add(leaf);
                }

                trunk.Add(branch);
            }

            return trunk;
        }

        public override void Write(Utf8JsonWriter writer, Trunk value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
