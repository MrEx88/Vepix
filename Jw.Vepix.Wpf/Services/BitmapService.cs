using Jw.Vepix.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Wpf.Services
{
    public enum BitmapEncoderType
    {
        BMP,
        GIF,
        JPEG,
        PNG,
        TIFF,
        WMP
    }

    public static class BitmapService
    {
        /// <summary>
        /// Converts byte array to a BitmapImage.
        /// </summary>
        /// <param name="bytes">byte array of image</param>
        /// <returns>Instance of BitmapImage</returns>
        public static BitmapImage ConvertByteArrayToBitmapImage(byte[] bytes)
        {
            var image = new BitmapImage();
            using (var stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
            }

            return image;
        }

        /// <summary>
        /// Converts Dictionary of filenames and byte arrays of images to a List of Picture
        /// </summary>
        /// <param name="files">Dictionary of key filename and value byte arrays</param>
        /// <returns>List of Picture</returns>
        public static async Task<List<Picture>> ConvertBytesListToPictureList(Dictionary<string, byte[]> files)
        {
            var pictures = new List<Picture>();
            await Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var file in files)
                    {
                        var bitmapImage = new BitmapImage();
                        var stream = new MemoryStream(file.Value);
                        stream.Seek(0, SeekOrigin.Begin);
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();
                        var picture = new Picture(bitmapImage, file.Key);

                        pictures.Add(picture);
                    }
                });
            });

            return pictures;
        }

        /// <summary>
        /// Crops a picture to the specified size and encoder.
        /// </summary>
        /// <param name="picture">The picture to crop</param>
        /// <param name="rect">The size to crop the image to</param>
        /// <param name="encoderType">The encoder type</param>
        /// <returns>The cropped Image</returns>
        public static BitmapImage CropPreview(Picture picture, Int32Rect rect, BitmapEncoderType encoderType)
        {
            var croppedImage = new CroppedBitmap(picture.BitmapImage, rect);
            return ConvertCroppedBitmapToBitMapImage(croppedImage, encoderType);
        }

        private static BitmapImage ConvertCroppedBitmapToBitMapImage(BitmapSource croppedImage, BitmapEncoderType encoderType)
        {
            var encoder = CreateBitmapEncoder(encoderType);
            
            using (var stream = new MemoryStream())
            {
                var bitmapImage = new BitmapImage();
                encoder.Frames.Add(BitmapFrame.Create(croppedImage));
                encoder.Save(stream);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        internal static BitmapEncoder CreateBitmapEncoder(BitmapEncoderType encoderType)
        {
            switch (encoderType)
            {
                case BitmapEncoderType.BMP:
                    return new BmpBitmapEncoder();
                case BitmapEncoderType.GIF:
                    return new GifBitmapEncoder();
                case BitmapEncoderType.PNG:
                    return new PngBitmapEncoder();
                case BitmapEncoderType.TIFF:
                    return new TiffBitmapEncoder();
                case BitmapEncoderType.WMP:
                    return new WmpBitmapEncoder();
                default:
                    return new JpegBitmapEncoder();
            }
        }
    }
}
