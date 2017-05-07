using Jw.Vepix.Common;
using Jw.Vepix.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Wpf.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Gets the bytes from the file name.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The byte array of the file</returns>
        byte[] GetFileBytes(string fileName);

        /// <summary>
        /// Gets the bytes from the file name.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The byte array of the file</returns>
        Task<byte[]> GetFileBytesAsync(string fileName);

        /// <summary>
        /// Gets the byte arrays from the file names.
        /// </summary>
        /// <param name="fileNames">The file names</param>
        /// <returns>A List of FileBytes</returns>
        Task<List<FileBytes>> GetFilesBytesAsync(List<string> fileNames);

        /// <summary>
        /// Gets the image file paths that have matched the criteria of the parameters.
        /// </summary>
        /// <param name="folderPath">The path to search in</param>
        /// <param name="searchPattern">The file filters to use (i.e. "*.jpg")</param>
        /// <param name="option">The SearchOption Enum to use</param>
        /// <returns>A List of FileBytes</returns>
        Task<List<FileBytes>> GetFilesBytesFromDirectoryAsync(string folderPath, List<string> searchPattern, SearchOption option);

        /// <summary>
        /// Gets the image file paths that have matched the criteria of the parameters.
        /// </summary>
        /// <param name="folderPath">The path to search in</param>
        /// <param name="searchPattern">The file pattern to use (i.e. "*.jpg")</param>
        /// <param name="option">The SearchOption Enum to use</param>
        /// <returns>A List of the image file paths</returns>
        Task<List<string>> GetFileNamesFromDirectoryAsync(string folderPath, List<string> searchPattern, SearchOption option);

        /// <summary>
        /// Changes the name of the file.
        /// </summary>
        /// <param name="oldName">The old name of the file</param>
        /// <param name="newName">The name to change to file to</param>
        /// <returns>True if name changed successfully.</returns>
        bool ChangeFileName(string oldName, string newName);

        /// <summary>
        /// Checks if the string has a valid file name.
        /// </summary>
        /// <param name="fileName">The file name to check</param>
        /// <returns>True if string is a valid file name</returns>
        bool IsValidFileName(string fileName);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="fileName">The file name to delete</param>
        /// <returns>True if file deleted successfully</returns>
        bool DeleteFile(string fileName);

        /// <summary>
        /// Saves a bitmap image with a new file name.
        /// </summary>
        /// <param name="bitmapImage">Bitmap image to save</param>
        /// <param name="encoderType">Bitmap encoder type</param>
        /// <returns>True if file was saved successfully</returns>
        bool SaveImageAs(BitmapImage bitmapImage, BitmapEncoderType encoderType);

        //todo: see what is the best way to implement this
        /// <summary>
        /// Overwrites a bitmap image file.
        /// </summary>
        /// <param name="bitmapImage">Bitmap image to save</param>
        /// <param name="encoderType">Bitmap encoder type</param>
        /// <returns>True if the file was overwritten successfully</returns>
        bool OverwriteImage(BitmapImage bitmapImage, string fullFileName, BitmapEncoderType encoderType);
    }
}
