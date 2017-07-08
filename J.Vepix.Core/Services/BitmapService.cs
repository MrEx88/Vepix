using J.Vepix.Core.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace J.Vepix.Core.Services
{
    public class BitmapService : IBitmapService
    {
        public BitmapImage CreateBitmapImage(byte[] bytes)
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

        public BitmapImage CreateBitmapImage(string fileName)
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

        public BitmapImage Crop(BitmapImage bitmapImage, Int32Rect rect, BitmapEncoderType encoderType)
        {
            var croppedImage = new CroppedBitmap(bitmapImage, rect);
            return ConvertCroppedBitmapToBitmapImage(croppedImage, encoderType);
        }

        //todo: I may want to make this public
        private BitmapImage ConvertCroppedBitmapToBitmapImage(CroppedBitmap croppedImage, BitmapEncoderType encoderType)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = EncoderService.CreateEncoder(encoderType, croppedImage);
                encoder.Save(stream);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage, BitmapEncoderType encoderType)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                var encoder = EncoderService.CreateEncoder(encoderType, bitmapImage);
                encoder.Save(outStream);
                var bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
