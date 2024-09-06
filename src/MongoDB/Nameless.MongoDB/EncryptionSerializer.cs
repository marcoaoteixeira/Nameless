using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Nameless.MongoDB;

public sealed class EncryptionSerializer : SerializerBase<string?> {
    private static readonly byte[] Default_Key = [0x38, 0xc4, 0x18, 0x22, 0x5b, 0xf2, 0xec, 0x9e];
    private static readonly byte[] Default_IV = [0x93, 0xdf, 0x93, 0x74, 0x43, 0xe0, 0x1b, 0x16];

    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionSerializer()
        : this(Default_Key, Default_IV) { }

    public EncryptionSerializer(byte[] key, byte[] iv) {
        Prevent.Argument.NullOrEmpty(key, nameof(key));
        Prevent.Argument.NullOrEmpty(iv, nameof(iv));

        if (key.Length != 8) { throw new ArgumentException("Parameter must be an array of 8 positions.", nameof(key)); }
        if (iv.Length != 8) { throw new ArgumentException("Parameter must be an array of 8 positions.", nameof(iv)); }

        _key = key;
        _iv = iv;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string? value) {
        if (!string.IsNullOrWhiteSpace(value)) {
            value = Crypt(_key, _iv, value);
            context.Writer.WriteString(value);
        } else { context.Writer.WriteNull(); }
    }

    public override string? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
        if (context.Reader.CurrentBsonType == BsonType.Null) {
            context.Reader.ReadNull();
            return null;
        }

        var value = context.Reader.ReadString();
        return Decrypt(_key, _iv, value);
    }

    private static string Crypt(byte[] key, byte[] iv, string value) {
        using var algorithm = DES.Create();
        using var transform = algorithm.CreateEncryptor(key, iv);

        var input = Encoding.Unicode.GetBytes(value);
        var output = transform.TransformFinalBlock(input, 0, input.Length);

        return Convert.ToBase64String(output);
    }

    private static string Decrypt(byte[] key, byte[] iv, string value) {
        using var algorithm = DES.Create();
        using var transform = algorithm.CreateDecryptor(key, iv);

        var input = Convert.FromBase64String(value);
        var output = transform.TransformFinalBlock(input, 0, input.Length);

        return Encoding.Unicode.GetString(output);
    }
}