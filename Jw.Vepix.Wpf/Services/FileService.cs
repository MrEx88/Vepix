using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jw.Vepix.Wpf.Services
{
    public class FileService : IFileService
    {
        /// <summary>
        /// Gets the image file paths that have matched the criteria of the parameters.
        /// </summary>
        /// <param name="path">The path to search in</param>
        /// <param name="searchPattern">The file filters to use (i.e. "*.jpg")</param>
        /// <param name="option">The SearchOption Enum to use</param>
        /// <returns>A List of the image file paths</returns>
        public async Task<Dictionary<string, byte[]>> GetFilesAndBytesFromDirectoryAsync(string path, List<string> searchPattern, SearchOption option)
        {
            var files = new List<string>();
            if (Directory.Exists(path))
            {
                await Task.Factory.StartNew(() =>
                    searchPattern.ForEach( sp =>
                        files.AddRange(Directory.GetFiles(path, sp, option))));
            }

            return await ReadBytesFromFilesAsync(files);
        }

        /// <summary>
        /// Changes the name of the file.
        /// </summary>
        /// <param name="oldName">The old name of the file</param>
        /// <param name="newName">The name to change to file to</param>
        public void ChangeFileName(string oldName, string newName)
        {
            File.Move(oldName, newName);
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="fileName">The file name to delete</param>
        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        /// <summary>
        /// Checks if the string has a valid file name.
        /// </summary>
        /// <param name="fileName">The file name to check</param>
        /// <returns>True if string is a valid file name</returns>
        public bool IsValidFileName(string fileName)
        {
            string invalidCharacters = "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]";
            return !(new Regex(invalidCharacters).IsMatch(fileName));
        }

        public byte[] ReadBytesFromFile(string file)
            => File.ReadAllBytes(file);

        public async Task<Dictionary<string, byte[]>> ReadBytesFromFilesAsync(List<string> files)
        {
            //todo: i think i will change this return type to just List<byte[]>
            var bytes = new Dictionary<string, byte[]>();
            await Task.Factory.StartNew(() =>
                files.ForEach(file => bytes.Add(file, ReadBytesFromFile(file))));

            return bytes;
        }
    }
}
