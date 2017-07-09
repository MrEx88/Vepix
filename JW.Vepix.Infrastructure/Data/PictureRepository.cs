using JW.Vepix.Core.Extensions;
using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Infrastructure.Data
{
    public class PictureRepository : IPictureRepository
    {
        public PictureRepository(IFileService fileService, IBitmapService bitmapService)
        {
            _fileService = fileService;
            _bitmapService = bitmapService;
        }

        //todo do I need to put async/await keywords here; what is the difference
        public Task<Picture> GetPictureAsync(string pictureFileName) =>
            Task.Factory.StartNew(() =>
                new Picture(_bitmapService.CreateBitmapImage(
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
                    _supportedPicturesPatterns.ToArray());

        public async Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath,
            SearchOption option = SearchOption.TopDirectoryOnly, params string[] searchPattern)
        {
            var fileBytes = await _fileService.GetFilesBytesFromDirectoryAsync(folderPath,
                searchPattern.ToList(), option);
            return await LoadPicturesAsync(fileBytes);
        }

        public Picture GetCroppedImage(Picture picture, Int32Rect rect)
        {
            var bitmap = _bitmapService.Crop(picture.BitmapImage, rect, picture.FileExtension.ToEncoderType());
            return new Picture(bitmap, picture.FullFileName);
        }

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

        public bool TryOverwrite(Picture picture)
        {
            //todo: handle exceptions
            //todo: overwriting is not working
            bool result = true;
            if (TryDelete(picture.FullFileName))
            {
                var bitmap = _bitmapService.ConvertBitmapImageToBitmap(picture.BitmapImage, picture.FileExtension.ToEncoderType());
                bitmap.Save(picture.FullFileName);
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
                    pictures.Add(new Picture(_bitmapService.CreateBitmapImage(file.Bytes),
                                             file.FullFileName));
                });
                return pictures;
            });
        }

        public bool TrySaveAs(BitmapImage image, BitmapEncoderType encoderType) =>
            _fileService.SaveImageAs(image, encoderType);

        private IFileService _fileService;
        private IBitmapService _bitmapService;
        private readonly List<string> _supportedPicturesPatterns = new List<string>
        {
            "*.jpg", "*.png", "*.gif", "*.bmp", "*.wmp", "*.tiff"
        };
    }
}
