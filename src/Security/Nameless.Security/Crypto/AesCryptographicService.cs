﻿using System.Security.Cryptography;
using Nameless.Security.Options;

namespace Nameless.Security.Crypto {
    public sealed class AesCryptographicService : ICryptographicService {
        #region Private Read-Only Fields

        private readonly AesCryptoOptions _options;

        #endregion

        #region Public Constructors

        public AesCryptographicService()
            : this(AesCryptoOptions.Default) { }

        public AesCryptographicService(AesCryptoOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region ICryptographicService Members

        public byte[] Encrypt(Stream stream) {
            Guard.Against.Null(stream, nameof(stream));

            if (!stream.CanRead) { throw new InvalidOperationException("Can't read the stream."); }
            if (stream.Length == 0) { return []; }

            var text = stream.ToText(_options.Encoding);
            var iv = new byte[16];

            using var aes = Aes.Create();
            aes.Key = _options.Encoding.GetBytes(_options.Key);
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(text);

            return memoryStream.ToArray();
        }

        public byte[] Decrypt(Stream stream) {
            Guard.Against.Null(stream, nameof(stream));

            if (!stream.CanRead) { throw new InvalidOperationException("Can't read the stream."); }
            if (stream.Length == 0) { return []; }

            var text = stream.ToText(_options.Encoding);
            var iv = new byte[16];

            var buffer = _options.Encoding.GetBytes(text);
            using var aes = Aes.Create();
            aes.Key = _options.Encoding.GetBytes(_options.Key);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            var result = streamReader.ReadToEnd();

            return _options.Encoding.GetBytes(result);
        }

        #endregion
    }
}
