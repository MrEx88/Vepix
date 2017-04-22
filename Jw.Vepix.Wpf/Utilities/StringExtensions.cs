using System.IO;

namespace Jw.Vepix.Wpf.Utilities
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
    }
}
