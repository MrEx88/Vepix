using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Core.Models
{
    public class Picture : ObjectBase
    {
        public Guid Guid { get; }

        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                if (value != _bitmapImage)
                {
                    _bitmapImage = value;

                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => Width);
                    NotifyPropertyChanged(() => Height);
                    NotifyPropertyChanged(() => Dimensions);
                }
            }
        }

        public string FullFileName
        {
            get { return _fullFileName; }
            set
            {
                if(value != _fullFileName)
                {
                    _fullFileName = value;

                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => FileName);
                    NotifyPropertyChanged(() => ImageName);
                    NotifyPropertyChanged(() => FolderPath);
                    //todo: how to deal with FileSize; maybe bitmapimage has a filesize property
                    NotifyPropertyChanged(() => FolderName);
                    NotifyPropertyChanged(() => FileExtension);
                }
            }
        } 

        public string FileName => Path.GetFileName(FullFileName);
        public string ImageName => Path.GetFileNameWithoutExtension(FullFileName);
        public string FolderPath => Path.GetDirectoryName(FullFileName) + "\\";
        public string FolderName => new DirectoryInfo(FolderPath).Name;
        public decimal FileSize => Math.Round((decimal)(new FileInfo(FullFileName).Length / 1024) / 1024, 2);
        public string FileExtension => Path.GetExtension(FullFileName);
        public int Width => BitmapImage.PixelWidth;
        public int Height => BitmapImage.PixelHeight;
        public string Dimensions => string.Format("W:{0}xH:{1}", Width.ToString(), Height.ToString());

        public Picture(BitmapImage bitmapImage, string fullFileName)
        {
            Guid = Guid.NewGuid();
            BitmapImage = bitmapImage;
            FullFileName = fullFileName;
            IsDirty = false;
        }

        private BitmapImage _bitmapImage;
        private string _fullFileName;
    }
}
