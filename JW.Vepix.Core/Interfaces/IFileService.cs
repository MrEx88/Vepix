using JW.Vepix.Core.Models;
using JW.Vepix.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Core.Interfaces
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
        /// <param name="searchPatterns">The file filters to use (i.e. "*.jpg")</param>
        /// <param name="option">The SearchOption Enum to use</param>
        /// <returns>A List of FileBytes</returns>
        Task<List<FileBytes>> GetFilesBytesFromDirectoryAsync(string folderPath, List<string> searchPatterns,
                                                              SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Gets the image file paths that have matched the criteria of the parameters.
        /// </summary>
        /// <param name="folderPath">The path to search in</param>
        /// <param name="searchPatterns">The file pattern to use (i.e. "*.jpg")</param>
        /// <param name="option">The SearchOption Enum to use</param>
        /// <returns>A List of the image file paths</returns>
        Task<List<string>> GetFileNamesFromDirectoryAsync(string folderPath, List<string> searchPatterns,
                                                          SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Changes the name of the file.
        /// </summary>
        /// <param name="oldFileName">The name of the old file</param>
        /// <param name="newFileName">The new file name</param>
        /// <returns>A TryResult instance</returns>
        TryResult ChangeFileName(string oldFileName, string newFileName);

        /// <summary>
        /// Copies the file to another location.
        /// </summary>
        /// <param name="sourceFileName">The name of the file to copy</param>
        /// <param name="destinationFolderPath">The folder to copy to</param>
        /// <returns>A TryResult instance</returns>
        TryResult CopyTo(string sourceFileName, string destinationFolderPath);

        /// <summary>
        /// Changes the name of the file.
        /// </summary>
        /// <param name="sourceFileName">The name to move to file</param>
        /// <param name="destinationFolderPath">The name of the folder to move to</param>
        /// <returns>A TryResult instance</returns>
        TryResult MoveTo(string sourceFileName, string destinationFolderPath);

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
        /// <returns>A TryResult instance</returns>
        TryResult DeleteFile(string fileName);

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
