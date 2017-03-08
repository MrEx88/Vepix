using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using Jw.Data;
using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public static class BitmapService
    {
        public static BitmapImage ConvertByteArrayToBitmapImage(Byte[] bytes)
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
    }
}
