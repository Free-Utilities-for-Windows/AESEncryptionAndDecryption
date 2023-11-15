using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AESEncryptionAndDecryption.Base
{
    public abstract class Cipher
    {
        protected string Encrypt(string message, params object[] keys)
        {
            var provider = CryptoProvider(keys);

            var encryptor = provider.CreateEncryptor(provider.Key, provider.IV);

            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(message);
            }

            var encrypted = memoryStream.ToArray();

            cryptoStream.Close();
            memoryStream.Close();

            return Convert.ToBase64String(encrypted);
        }

        protected string Decrypt(string message, params object[] keys)
        {
            var provider = CryptoProvider(keys);

            var bytes = Convert.FromBase64String(message);

            var decryptor = provider.CreateDecryptor(provider.Key, provider.IV);

            var memoryStream = new MemoryStream(bytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            string plaintext;
            using (var reader = new StreamReader(cryptoStream))
            {
                plaintext = reader.ReadToEnd();
            }

            cryptoStream.Close();
            memoryStream.Close();

            return plaintext;
        }

        private SymmetricAlgorithm CryptoProvider(object[] keys)
        {
            SymmetricAlgorithm cipher = Aes.Create();

            cipher.Mode = ValidateAndCastCipherMode(keys[0]);

            byte[] keyBytes = Convert.FromBase64String(ValidateAndCastKey(keys[1]));
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
            {
                throw new Exception("Key must be 16, 24, or 32 bytes for AES encryption");
            }

            cipher.Key = keyBytes;

            byte[] ivBytes = Convert.FromBase64String(ValidateAndCastIV(keys[2]));
            if (ivBytes.Length != 16)
            {
                throw new Exception("IV must be 16 bytes for AES encryption");
            }

            cipher.IV = ivBytes;
            
            return cipher;
        }

        private CipherMode ValidateAndCastCipherMode(object m)
        {
            if (m is CipherMode mode)
            {
                return mode;
            }
            else
            {
                throw new ArgumentException("Wrong cipher mode");
            }
        }

        private string ValidateAndCastKey(object m)
        {
            if (!(m is string))
            {
                throw new ArgumentException("Wrong key type");
            }

            string key = m as string;
            int bytes = Encoding.UTF8.GetByteCount(key);

            if (!(new int[] { 16, 24, 32 }.Contains(bytes)))
            {
                throw new ArgumentException("Bad key size for AES. Key size must be 16, 24, or 32 bytes.");
            }

            return key;
        }

        private string ValidateAndCastIV(object m)
        {
            if (!(m is string))
            {
                throw new ArgumentException("Wrong IV type");
            }

            string iv = m as string;
            byte[] bytes = Convert.FromBase64String(iv);

            if (bytes.Length != 16)
            {
                throw new ArgumentException("Bad IV size for AES. IV size must be 16 bytes.");
            }

            return iv;
        }
    }
}