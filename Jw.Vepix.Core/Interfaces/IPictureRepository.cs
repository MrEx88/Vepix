using Jw.Vepix.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Core.Interfaces
{
    public interface IPictureRepository
    {
        /// <summary>
        /// Gets picture from file name.
        /// </summary>
        /// <param name="pictureFileName">The full picture file name</param>
        /// <returns>A Picture instance</returns>
        Task<Picture> GetPictureAsync(string pictureFileName);

        /// <summary>
        /// Gets picture from file name.
        /// </summary>
        /// <param name="pictureFileNames">The files to get pictures from</param>
        /// <returns>A List of Picture</returns>
        Task<List<Picture>> GetPicturesAsync(string[] pictureFileNames);

        /// <summary>
        /// Gets pictures from a list of files.
        /// </summary>
        /// <param name="pictureFileNames">The files to get pictures from</param>
        /// <returns>A List of Picture</returns>
        Task<List<Picture>> GetPicturesAsync(List<string> pictureFileNames);

        /// <summary>
        /// Gets pictures from the selected files in windows explorer.
        /// </summary>
        /// <param name="folderPath">The folder to search pictures for</param>
        /// <param name="option">The SearchOption enum to use</param>
        /// <returns>A List of Picture</returns>
        Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath, SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Gets pictures from the selected files in windows explorer.
        /// </summary>
        /// <param name="folderPath">The folder to search pictures for</param>
        /// <param name="option">The SearchOption enum to use</param>
        /// <param name="searchPattern">File filters to use (e.g "*.jpg")</param>
        /// <returns>A List of Picture</returns>
        Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath, SearchOption option = SearchOption.TopDirectoryOnly, params string[] searchPattern);

        /// <summary>
        /// Gets pictures from the selected files in windows explorer.
        /// </summary>
        /// <param name="folderPath">The folder to search pictures for</param>
        /// <param name="option">The SearchOption enum to use</param>
        /// <returns>A List of picture file names</returns>
        Task<List<string>> GetFileNamesAsync(string folderPath, SearchOption option = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Trys to change the name of the picture.
        /// </summary>
        /// <param name="picture">The picture to be changed</param>
        /// <param name="newName">The new name of the picture</param>
        /// <returns>True if name has been changed</returns>
        bool TryChangePictureName(Picture picture, string newName);

        /// <summary>
        /// Trys to delete the specified picture.
        /// </summary>
        /// <param name="pictureFileName">The picture to delete</param>
        /// <returns>True if name has been changed</returns>
        bool TryDelete(string pictureFileName);

        /// <summary>
        /// Trys to overwrite the specified picture.
        /// </summary>
        /// <param name="croppedImage">The new picture to overwrite</param>
        /// <param name="fullFileName">The file name of the picture</param>
        /// <param name="encoderType">The encoder type</param>
        /// <returns>True if the image file has been overwritten</returns>
        bool TryOverWrite(BitmapImage croppedImage, string fullFileName, BitmapEncoderType encoderType);

        /// <summary>
        /// Trys to save the image as a new image.
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="encoderType">The encoder type</param>
        /// <returns>True if the image file has been created</returns>
        bool TrySaveAs(BitmapImage image, BitmapEncoderType encoderType);
    }
}
