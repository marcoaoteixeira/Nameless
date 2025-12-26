using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Nameless.MongoDB.Security;

/// <summary>
/// Serializer for encrypting and decrypting string values using DES algorithm.
/// </summary>
public sealed class EncryptionSerializer : SerializerBase<string?> {
    private static readonly byte[] DefaultKey = [0x38, 0xc4, 0x18, 0x22, 0x5b, 0xf2, 0xec, 0x9e];
    private static readonly byte[] DefaultIv = [0x93, 0xdf, 0x93, 0x74, 0x43, 0xe0, 0x1b, 0x16];

    private readonly byte[] _iv;
    private readonly byte[] _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptionSerializer"/>.
    /// </summary>
    public EncryptionSerializer()
        : this(DefaultKey, DefaultIv) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptionSerializer"/>.
    /// </summary>
    /// <param name="key">The secret key.</param>
    /// <param name="iv">The initialization vector.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="key"/> or
    ///     <paramref name="iv"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="key"/> or
    ///     <paramref name="iv"/> is empty array.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <paramref name="key"/> or
    ///     <paramref name="iv"/> length is not exactly 8 positions.
    /// </exception>
    public EncryptionSerializer(byte[] key, byte[] iv) {
        Guard.Against.NullOrEmpty(key);
        Guard.Against.NullOrEmpty(iv);

        if (key.Length != 8) {
            throw new ArgumentException(message: "Parameter must be an array of 8 positions.", nameof(key));
        }

        if (iv.Length != 8) {
            throw new ArgumentException(message: "Parameter must be an array of 8 positions.", nameof(iv));
        }

        _key = key;
        _iv = iv;
    }

    /// <inheritdoc />
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string? value) {
        if (string.IsNullOrWhiteSpace(value)) {
            context.Writer.WriteNull();

            return;
        }

        value = Crypt(_key, _iv, value);
        context.Writer.WriteString(value);
    }

    /// <inheritdoc />
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
        var output = transform.TransformFinalBlock(input, inputOffset: 0, input.Length);

        return Convert.ToBase64String(output);
    }

    private static string Decrypt(byte[] key, byte[] iv, string value) {
        using var algorithm = DES.Create();
        using var transform = algorithm.CreateDecryptor(key, iv);

        var input = Convert.FromBase64String(value);
        var output = transform.TransformFinalBlock(input, inputOffset: 0, input.Length);

        return Encoding.Unicode.GetString(output);
    }
}