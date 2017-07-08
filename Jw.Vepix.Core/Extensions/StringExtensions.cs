using Jw.Vepix.Core.Services;
using System.IO;

namespace Jw.Vepix.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSubFolderOf(this string folder, string otherFolder)
        {
            var isChild = false;
            var directoryInfo = new DirectoryInfo(folder);
            var otherDirectoryInfo = new DirectoryInfo(otherFolder);

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

        public static string ToFilesFolderName(this string file)
            => Path.GetDirectoryName(file).ToFoldersName();

        public static string ToFoldersName(this string folder)
            => new DirectoryInfo(folder).Name;

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
                    throw new FileFormatException($"{fileExtension} is not a valid bitmap encoder type");
            }
        }
    }
}
