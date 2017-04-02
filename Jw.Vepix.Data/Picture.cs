using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Data
{
    public class Picture
    {
        public BitmapImage BitmapImage { get; set; }
        public string FullFileName { get; set; } //? should this be set publically. Also what about moving the file to another folder??
        public string ImageName => Path.GetFileNameWithoutExtension(FullFileName);
        public string FolderPath => Path.GetDirectoryName(FullFileName) + "\\";
        public decimal FileSize => Math.Round((decimal)(new FileInfo(FullFileName).Length / 1024) / 1024, 2);
        public string FileExtension => Path.GetExtension(FullFileName);
        public int Width => BitmapImage.PixelWidth;
        public int Height => BitmapImage.PixelHeight;
        public string Dimensions => string.Format("W:{0}xH:{1}", Width.ToString(), Height.ToString());

        public Picture(BitmapImage bitmapImage, string fullFileName)
        {
            BitmapImage = bitmapImage;
            FullFileName = fullFileName;
        }
    }
}
