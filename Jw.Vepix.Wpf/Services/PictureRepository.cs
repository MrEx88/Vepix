using Jw.Vepix.Common;
using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Wpf.Services
{
    public class PictureRepository : IPictureRepository
    {
        public PictureRepository()
        {
            _fileService = new FileService();
        }

        //todo do I need to put async/await keywords here; what is the difference
        public Task<Picture> GetPictureAsync(string pictureFileName) =>
            Task.Factory.StartNew(() =>
                new Picture(BitmapService.ConvertByteArrayToBitmapImage(
                    _fileService.GetFileBytes(pictureFileName)), pictureFileName));

        public Task<List<Picture>> GetPicturesAsync(string[] pictureFileNames) =>
            GetPicturesAsync(pictureFileNames.ToList());

        public async Task<List<Picture>> GetPicturesAsync(List<string> pictureFileNames)
        {
            var fileBytes = await _fileService.GetFilesBytesAsync(pictureFileNames);
            return await LoadPicturesAsync(fileBytes);
        }

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
                var console = VepixConsoleParser.ConsoleInstance();
                foreach (var dir in console.TopDirectories)
                {
                    pictures.AddRange(GetPicturesFromFolderAsync(dir, 
                        SearchOption.TopDirectoryOnly, console.SearchPatterns.ToArray()).Result);
                }
                foreach (var dir in console.AllDirectories)
                {
                    pictures.AddRange(GetPicturesFromFolderAsync(dir,
                        SearchOption.AllDirectories, console.SearchPatterns.ToArray()).Result);
                }
            });

            return pictures;
        }

        public async Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath,
            SearchOption option = SearchOption.TopDirectoryOnly) =>
                await GetPicturesFromFolderAsync(folderPath, option,
                    _supportedImagesFilterList.ToArray());

        public async Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath,
            SearchOption option = SearchOption.TopDirectoryOnly, params string[] searchPattern)
        {
            var fileBytes = await _fileService.GetFilesBytesFromDirectoryAsync(folderPath,
                searchPattern.ToList(), option);
            return await LoadPicturesAsync(fileBytes);
        }

        public async Task<List<string>> GetFileNamesAsync(string folderPath, 
            SearchOption option = SearchOption.TopDirectoryOnly) =>
                await _fileService.GetFileNamesFromDirectoryAsync(folderPath,
                                            _supportedImagesFilterList, option);

        public bool TryChangePictureName(Picture picture, string newName)
        {
            //todo: handle exceptions
            bool result = true;
            _fileService.ChangeFileName(picture.FullFileName, 
                picture.FolderPath + newName + picture.FileExtension);
            return result;
        }

        public bool TryDelete(string fileName)
        {
            //todo: handle exceptions
            bool result = true;
            _fileService.DeleteFile(fileName);
            return result;
        }

        public bool TryOverWrite(BitmapImage croppedImage, string fullFileName,
            BitmapEncoderType encoderType)
        {
            //todo: handle exceptions
            //todo: overwriting is not working
            bool result = true;
            if (TryDelete(fullFileName))
            {
                var bitmap = Bitmapper.ConvertBitmapImageToBitmap(croppedImage, encoderType);
                bitmap.Save(fullFileName);
                //_fileService.SaveImage(croppedImage, fullFileName, encoderType);
            }

            return result;
        }

        private Task<List<Picture>> LoadPicturesAsync(List<FileBytes> fileBytes)
        {
            return Task.Factory.StartNew<List<Picture>>(() =>
            {
                List<Picture> pictures = new List<Picture>();
                Parallel.ForEach(fileBytes, file =>
                {
                    pictures.Add(new Picture(BitmapService.ConvertByteArrayToBitmapImage(file.Bytes),
                                             file.FullFileName));
                });
                return pictures;
            });
        }

        public bool TrySaveAs(BitmapImage image, BitmapEncoderType encoderType) =>
            _fileService.SaveImageAs(image, encoderType);

        private IFileService _fileService;
        private readonly List<string> _supportedImagesFilterList = new List<string>
        {
            "*.jpg", "*.png", "*.gif", "*.bmp", "*.wmp", "*.tiff"
        };
    }
}
