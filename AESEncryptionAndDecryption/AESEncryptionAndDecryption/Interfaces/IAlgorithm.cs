namespace AESEncryptionAndDecryption.Interfaces
{
    public interface IAlgorithm
    {
        string Title { get; set; }

        string Description { get; set; }

        string Encrypt(string message, params object[] keys);

        string Decrypt(string message, params object[] keys);
    }
}