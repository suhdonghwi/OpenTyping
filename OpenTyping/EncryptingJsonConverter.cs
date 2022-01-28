using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenTyping
{
    public class EncryptingJsonConverter : JsonConverter
    {
        private readonly byte[] _encryptionKeyBytes;

        public EncryptingJsonConverter(string encryptionKey)
        {
            if (encryptionKey == null)
            {
                throw new ArgumentNullException(nameof(encryptionKey));
            }

            // Hash the key to ensure it is exactly 256 bits long, as required by AES-256
            using (var sha = new SHA256Managed())
            {
                _encryptionKeyBytes =
                    sha.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
            {
                writer.WriteNull();
                return;
            }

            var buffer = Encoding.UTF8.GetBytes(stringValue);

            using (var inputStream = new MemoryStream(buffer, false))
            using (var outputStream = new MemoryStream())
            using (var aes = new AesManaged
            {
                Key = _encryptionKeyBytes
            })
            {
                var iv = aes.IV; // first access generates a new IV
                outputStream.Write(iv, 0, iv.Length);
                outputStream.Flush();

                var encryptor = aes.CreateEncryptor(_encryptionKeyBytes, iv);
                using (var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }

                writer.WriteValue(Convert.ToBase64String(outputStream.ToArray()));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            if (string.IsNullOrEmpty(value))
            {
                return reader.Value;
            }

            try
            {
                var buffer = Convert.FromBase64String(value);

                using (var inputStream = new MemoryStream(buffer, false))
                using (var outputStream = new MemoryStream())
                using (var aes = new AesManaged
                {
                    Key = _encryptionKeyBytes
                })
                {
                    var iv = new byte[16];
                    var bytesRead = inputStream.Read(iv, 0, 16);
                    if (bytesRead < 16)
                    {
                        throw new CryptographicException("IV is missing or invalid.");
                    }

                    var decryptor = aes.CreateDecryptor(_encryptionKeyBytes, iv);
                    using (var cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }

                    var decryptedValue = Encoding.UTF8.GetString(outputStream.ToArray());
                    return decryptedValue;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}