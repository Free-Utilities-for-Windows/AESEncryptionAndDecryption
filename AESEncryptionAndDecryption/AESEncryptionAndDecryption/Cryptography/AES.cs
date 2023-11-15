using AESEncryptionAndDecryption.Base;
using AESEncryptionAndDecryption.Interfaces;

namespace AESEncryptionAndDecryption.Cryptography
{
    public class AES : Cipher, IAlgorithm
    {
        public string Title { get; set; }
        public string Description { get; set; }
        
        public AES()
        {
            Title = "AES";
            Description = "Advanced Encryption Standard";
        }

        public new string Encrypt(string message, params object[] keys)
        {
            return base.Encrypt(message, keys);
        }

        public new string Decrypt(string message, params object[] keys)
        {
            return base.Decrypt(message, keys);
        }
    }
}