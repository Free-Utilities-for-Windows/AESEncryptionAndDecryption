using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using AESEncryptionAndDecryption.Controllers.Interfaces;

namespace AESEncryptionAndDecryption.Controllers
{
    public class FileController : IFileController
    {
        public async Task<StorageFile> Open(string filter = ".txt")
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(filter);

            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                throw new Exception("Operation cancelled.");
            }

            return file;
        }

        public async Task Save(StorageFile file, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var extension = file.FileType;

            if (extension.Equals(".txt"))
            {
                await FileIO.WriteTextAsync(file, text);
            }
            else if (extension.Equals(".pem"))
            {
                var buffer = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
                await FileIO.WriteBufferAsync(file, buffer);
            }
            else
            {
                var buffer = CryptographicBuffer.DecodeFromHexString(text);
                await FileIO.WriteBufferAsync(file, buffer);
            }
        }

        public async Task<StorageFile> SaveAs(string text, string filter = ".txt")
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            savePicker.FileTypeChoices.Add("Text File", new List<string>() { filter });
            savePicker.SuggestedFileName = "New Document";

            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                throw new Exception("Operation cancelled.");
            }

            await Save(file, text);

            return file;
        }
    }
}