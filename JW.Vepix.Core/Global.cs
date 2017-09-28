using System.Collections.Generic;
using System.Linq;

namespace JW.Vepix.Core
{
    public static class Global
    {
        public static readonly List<string> SUPPORTED_FILE_FORMATS = new List<string>
        {
            ".jpg", ".png", ".gif", ".bmp", ".wmp", ".tiff"
        };

        public static readonly List<string> ALL_SUPPORTED_PATTERNS = 
            SUPPORTED_FILE_FORMATS.Select(format => "*" + format).ToList();
    }
}
