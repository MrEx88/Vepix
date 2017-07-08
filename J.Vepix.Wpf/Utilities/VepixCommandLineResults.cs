using System.Collections.Generic;

namespace J.Vepix.Wpf.Utilities
{
    public class VepixCommandLineResults
    {
        public List<string> Folders { get; }
        public List<string> TreeFolders { get; }
        public List<string> SearchPatterns { get; }

        internal VepixCommandLineResults(List<string> folders, List<string> treeFolders, List<string> searchPatterns)
        {
            Folders = folders;
            TreeFolders = treeFolders;
            SearchPatterns = searchPatterns;
        }
    }
}
