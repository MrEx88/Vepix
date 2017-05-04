using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Common
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

    public static class Bitmapper
    {
        /// <summary>
        /// Converts byte array to a BitmapImage.
        /// </summary>
        /// <param name="bytes">byte array of image</param>
        /// <returns>Instance of BitmapImage</returns>
        public static BitmapImage CreateBitmapImage(byte[] bytes)
        {
            var image = new BitmapImage();
            using (var stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
            }

            return image;
        }

        /// <summary>
        /// Converts byte array to a BitmapImage.
        /// </summary>
        /// <param name="fileName">The picture file name</param>
        /// <returns>Instance of BitmapImage</returns>
        public static BitmapImage CreateBitmapImage(string fileName)
        {
            // todo: check if I need to use BeginInit() and EndInit() and when using
            //       ctor with Uri object.
            var image = new BitmapImage(new Uri(fileName))
            {
                CacheOption = BitmapCacheOption.OnLoad
            };
            image.Freeze();

            return image;
        }

        /// <summary>
        /// Crops a picture to the specified size and encoder.
        /// </summary>
        /// <param name="picture">The picture to crop</param>
        /// <param name="rect">The size to crop the image to</param>
        /// <param name="encoderType">The encoder type</param>
        /// <returns>The cropped Image</returns>
        public static BitmapImage Crop(BitmapImage picture, Int32Rect rect, BitmapEncoderType encoderType)
        {
            var croppedImage = new CroppedBitmap(picture, rect);
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

        public static BitmapEncoder CreateBitmapEncoder(BitmapEncoderType encoderType)
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
