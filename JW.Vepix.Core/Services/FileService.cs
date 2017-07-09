using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Core.Services
{
    public class FileService : IFileService
    {
        public async Task<List<FileBytes>> GetFilesBytesAsync(List<string> fileNames)
        {
            //todo maybe use parallel foreach here
            var bytes = new List<FileBytes>();
            await Task.Factory.StartNew(() =>
                fileNames.ForEach(async file => bytes.Add(new FileBytes(file,
                                            await GetFileBytesAsync(file)))));

            return bytes;
        }

        public async Task<List<string>> GetFileNamesFromDirectoryAsync(string folderPath,
            List<string> searchPatterns, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            var files = new List<string>();
            if (Directory.Exists(folderPath))
            {
                await Task.Factory.StartNew(() =>
                    files.AddRange(searchPatterns.SelectMany(searchPattern =>
                        Directory.EnumerateFiles(folderPath, searchPattern, option).ToList())));
            }

            return files;
        }

        public byte[] GetFileBytes(string fileName) => File.ReadAllBytes(fileName);

        public Task<byte[]> GetFileBytesAsync(string fileName) =>
            Task.Factory.StartNew(() => File.ReadAllBytes(fileName));

        public async Task<List<FileBytes>> GetFilesBytesFromDirectoryAsync(string folderPath,
            List<string> searchPatterns, SearchOption option = SearchOption.TopDirectoryOnly) =>
                await GetFilesBytesAsync(await GetFileNamesFromDirectoryAsync(folderPath, searchPatterns, option));

        public TryResult ChangeFileName(string sourceFileName, string destinationFileName)
        {
            var tryResult = new TryResult();
            tryResult.Try(() => File.Move(sourceFileName, destinationFileName));
            return tryResult;
        }

        public TryResult CopyTo(string sourceFileName, string destinationFolderPath)
        {
            var tryResult = new TryResult();
            var fileName = Path.GetFileName(sourceFileName);
            tryResult.Try(() => File.Copy(sourceFileName, $"{destinationFolderPath}\\{fileName}"));
            return tryResult;
        }

        public TryResult MoveTo(string sourceFileName, string destinationFolderPath)
        {
            var tryResult = new TryResult();
            var fileName = Path.GetFileName(sourceFileName);
            tryResult.Try(() => File.Move(sourceFileName, $"{destinationFolderPath}\\{fileName}"));
            return tryResult;
        }

        public TryResult DeleteFile(string fileName)
        {
            var tryResult = new TryResult();
            tryResult.Try(() => File.Delete(fileName));
            return tryResult;
        }

        public bool IsValidFileName(string fileName)
        {
            string invalidCharacters = "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]";
            return !(new Regex(invalidCharacters).IsMatch(fileName));
        }

        public bool OverwriteImage(BitmapImage bitmapImage, string fullFileName,
            BitmapEncoderType encoderType)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = fullFileName;
                var encoder = EncoderService.CreateEncoder(encoderType, bitmapImage);
                using (var fileStream = (FileStream)saveDialog.OpenFile())
                {
                    encoder.Save(fileStream);
                }
            }
            return true;
        }

        public bool SaveImageAs(BitmapImage bitmapImage, BitmapEncoderType encoderType)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Vepix: Save Picture As...";
                saveDialog.Filter = "Jpeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveDialog.ShowDialog();
                if (saveDialog.FileName != null && saveDialog.FileName != "")
                {
                    var encoder = EncoderService.CreateEncoder(encoderType, bitmapImage);
                    using (var fileStream = (FileStream)saveDialog.OpenFile())
                    {
                        encoder.Save(fileStream);
                    }
                }
            }

            return true;
        }
    }
}
