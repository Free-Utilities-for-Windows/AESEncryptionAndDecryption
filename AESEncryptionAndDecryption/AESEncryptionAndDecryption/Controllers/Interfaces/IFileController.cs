using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace AESEncryptionAndDecryption.Controllers.Interfaces
{
    public interface IFileController
    {
        Task<StorageFile> Open(string filter = "Text files (*.txt)|*.txt;|All files (*.*)|*.*");

        Task Save(StorageFile fileInfo, string content);

        Task<StorageFile> SaveAs(string text, string filter = "Text files (*.txt)|*.txt;|All files (*.*)|*.*");
    }
}