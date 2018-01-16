using JW.Vepix.Core;
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
        private IFileService _fileService;
        private IBitmapService _bitmapService;

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
                    Global.ALL_SUPPORTED_PATTERNS.ToArray());

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

        public TryResult TryChangePictureName(Picture picture, string newName) =>
            _fileService.ChangeFileName(picture.FullFileName, 
               $"{picture.FolderPath}\\{newName}{picture.FileExtension}");

        public TryResult TryCopy(Picture picture, string fullFolderPath) => 
            _fileService.CopyTo(picture.FullFileName, fullFolderPath);

        public TryResult TryMove(Picture picture, string fullFolderPath) =>
            _fileService.MoveTo(picture.FullFileName, fullFolderPath);

        public TryResult TryDelete(string fileName) => _fileService.DeleteFile(fileName);

        public TryResult TryOverwrite(Picture picture)
        {
            //todo: overwriting is not working
            var tryResult = TryDelete(picture.FullFileName);
            if (tryResult.Success.Value)
            {
                var bitmap = _bitmapService.ConvertBitmapImageToBitmap(picture.BitmapImage, picture.FileExtension.ToEncoderType());
                bitmap.Save(picture.FullFileName);
                //_fileService.SaveImage(croppedImage, fullFileName, encoderType);
            }

            return tryResult;
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
    }
}
