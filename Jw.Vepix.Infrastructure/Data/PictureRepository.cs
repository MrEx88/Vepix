using Jw.Vepix.Core;
using Jw.Vepix.Core.Interfaces;
using Jw.Vepix.Core.Models;
using Jw.Vepix.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Infrastructure.Data
{
    public class PictureRepository : IPictureRepository
    {
        public PictureRepository(IFileService fileService)
        {
            _fileService = fileService;
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

        public bool TryCopy(Picture picture, string fullFolderPath)
        {
            bool result = true;
            _fileService.CopyTo(fullFolderPath, picture.ImageName);
            return result;
        }

        public bool TryMove(Picture picture, string fullFolderPath)
        {
            bool result = true;
            _fileService.MoveTo(fullFolderPath, picture.ImageName);
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
