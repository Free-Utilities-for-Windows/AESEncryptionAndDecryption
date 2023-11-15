using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AESEncryptionAndDecryption.Controllers;
using AESEncryptionAndDecryption.Cryptography;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace AESEncryptionAndDecryption
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
 public sealed partial class MainPage : Page
    {
        private readonly FileController _fileController = new FileController();
        private readonly AES _aes = new AES();
        private StorageFile _selectedFile;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedFile = await _fileController.Open();
            FilePathTextBox.Text = _selectedFile?.Path;
        }

        private async void GenerateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var key = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(key);
                }

                var provider = System.Security.Cryptography.Aes.Create();
                provider.Key = key;
                KeyBox.Text = Convert.ToBase64String(provider.Key);
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred while generating the key: {ex.Message}",
                    CloseButtonText = "Ok",
                    Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 35))
                };

                await dialog.ShowAsync();
            }
        }

        private async void GenerateIVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var provider = System.Security.Cryptography.Aes.Create();
                provider.GenerateIV();
                var bytes = provider.IV;
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] %= 128;
                }

                GenerateIVBox.Text = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred while generating the IV: {ex.Message}",
                    CloseButtonText = "Ok",
                    Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 35))
                };

                await dialog.ShowAsync();
            }
        }

        private async void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == null)
            {
                return;
            }

            var text = await FileIO.ReadTextAsync(_selectedFile);
            var encryptedText = _aes.Encrypt(text, CipherMode.CBC, KeyBox.Text.Trim(), GenerateIVBox.Text.Trim());
            await _fileController.Save(_selectedFile, encryptedText);

            var dialog = new ContentDialog
            {
                Title = "Encryption completed successfully.",
                Content = "Your file has been encrypted.",
                CloseButtonText = "Ok",
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 35))
            };

            await dialog.ShowAsync();
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == null)
            {
                return;
            }

            var encryptedText = await FileIO.ReadTextAsync(_selectedFile);
            var decryptedText =
                _aes.Decrypt(encryptedText, CipherMode.CBC, KeyBox.Text.Trim(), GenerateIVBox.Text.Trim());
            await _fileController.Save(_selectedFile, decryptedText);

            var dialog = new ContentDialog
            {
                Title = "Decryption completed successfully.",
                Content = "Your file has been Decrypted.",
                CloseButtonText = "Ok",
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 35))
            };

            await dialog.ShowAsync();
        }
    }
}
