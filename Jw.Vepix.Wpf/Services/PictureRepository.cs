using Jw.Data;
using Jw.Vepix.Wpf.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jw.Vepix.Wpf.Services
{
    public class PictureRepository : IPictureRepository
    {
        public PictureRepository()
        {
            _fileService = new FileService();
        }

        /// <summary>
        /// Gets pictures from an array of files.
        /// </summary>
        /// <param name="files">The files to get pictures from</param>
        /// <returns>A List of pictures</returns>
        public async Task<List<Picture>> GetPicturesAsync(string[] files)
        {
            var fileBytes = await _fileService.ReadBytesFromFilesAsync(files.ToList());
            return await LoadPicturesAsync(fileBytes);
        }

        /// <summary>
        /// Gets pictures from the selected files in windows explorer.
        /// </summary>
        /// <param name="folderPath">The folder to search pictures for</param>
        /// <param name="option">The SearchOption enum to use</param>
        /// <returns>A List of pictures</returns>
        public async Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath, SearchOption option)
        {
            var fileBytes = await _fileService.GetFilesAndBytesFromDirectoryAsync(folderPath, _supportedImagesFilterList, option);
            return await LoadPicturesAsync(fileBytes);
        }

        /// <summary>
        /// Gets pictures from a folder.
        /// </summary>
        /// <param name="folderPath">The folder to search pictures for</param>
        /// <param name="option">The SearchOption enum to use</param>
        /// <param name="searchPattern">File filters to use (e.g "*.jpg")</param>
        /// <returns>A List of pictures</returns>
        public async Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath, SearchOption option, string[] searchPattern)
        {
            var fileBytes = await _fileService.GetFilesAndBytesFromDirectoryAsync(folderPath, searchPattern.ToList(), option);
            return await LoadPicturesAsync(fileBytes);
        }
        
        /// <summary>
        /// Gets pictures from the command line.
        /// </summary>
        /// <returns>A List of pictures</returns>
        public async Task<List<Picture>> GetPicturesFromCommandLineAsync()
        {
            // todo: Now I know the difference between file filters and search patterns
            // filefilters = "Image Files|*.jpg;*.jpeg;*.png;*.gif" (filter for image file format types)
            // search patterns = "1*.jpg" or "r*.*"
            // for command line, i will change search patterns to allow something like "*.*"
            // when getting the files here, I still need to do the image file filter 
            List<Picture> pictures = new List<Picture>();

            await Task.Factory.StartNew(() =>
            {
                var console = VepixConsole.Instance();
                foreach (var dir in console.TopDirectories)
                {
                    pictures.AddRange(GetPicturesFromFolderAsync(dir, SearchOption.TopDirectoryOnly, console.SearchPatterns.ToArray()).Result);
                }
                foreach (var dir in console.AllDirectories)
                {
                    pictures.AddRange(GetPicturesFromFolderAsync(dir, SearchOption.AllDirectories, console.SearchPatterns.ToArray()).Result);
                }
            });

            return pictures;
        }

        /// <summary>
        /// Trys to change the name of the picture.
        /// </summary>
        /// <param name="picture">The picture to be changed</param>
        /// <param name="newName">The new name of the picture</param>
        /// <returns>True if name has been changed</returns>
        public bool TryChangePictureName(Picture picture, string newName)
        {
            //todo: handle exceptions
            bool result = false;
            _fileService.ChangeFileName(picture.FullFileName, picture.FolderPath + newName + picture.FileExtension);
            result = true;
            return result;
        }

        /// <summary>
        /// Trys to delete the specified picture.
        /// </summary>
        /// <param name="picture">The picture to delete</param>
        /// <returns>True if name has been changed</returns>
        public bool TryDelete(Picture picture)
        {
            //todo: handle exceptions
            bool result = false;
            _fileService.DeleteFile(picture.FullFileName);
            result = true;
            return result;
        }

        private Task<List<Picture>> LoadPicturesAsync(Dictionary<string, byte[]> fileBytes)
        {
            return Task.Factory.StartNew<List<Picture>>(() =>
            {
                List<Picture> pictures = new List<Picture>();
                Parallel.ForEach(fileBytes, file =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        pictures.Add(new Picture(BitmapService.ConvertByteArrayToBitmapImage(file.Value), file.Key));
                    });
                });
                return pictures;
            });
        }

        private IFileService _fileService;
        private List<string> _supportedImagesFilterList = new List<string> { "*.jpg", "*.png", "*.gif" };
    }
}
