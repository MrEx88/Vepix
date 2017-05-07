using System.IO;

namespace Jw.Vepix.Common
{
    public static class StringExtensions
    {
        public static bool IsSubDirectoryOf(this string directory, string otherDirectory)
        {
            var isChild = false;
            var directoryInfo = new DirectoryInfo(directory);
            var otherDirectoryInfo = new DirectoryInfo(otherDirectory);

            while (directoryInfo.Parent != null)
            {
                if (directoryInfo.Parent.FullName == otherDirectoryInfo.FullName)
                {
                    isChild = true;
                    break;
                }
                else
                {
                    directoryInfo = directoryInfo.Parent;
                }
            }

            return isChild;
        }

        public static BitmapEncoderType ToEncoderType(this string fileExtension)
        {
            switch (fileExtension)
            {
                case ".bmp":
                    return BitmapEncoderType.BMP;
                case ".gif":
                    return BitmapEncoderType.GIF;
                case ".jpg":
                    return BitmapEncoderType.JPEG;
                case ".PNG":
                    return BitmapEncoderType.PNG;
                case ".TIFF":
                    return BitmapEncoderType.TIFF;
                case ".WMP":
                    return BitmapEncoderType.WMP;
                default:
                    throw new FileFormatException($"{fileExtension} not a valid bitmap encoder type");
            }
        }
    }
}
