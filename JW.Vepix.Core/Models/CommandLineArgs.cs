using System.Collections.Generic;

namespace JW.Vepix.Core.Utilities
{
    public class CommandLineArgs
    {
        private static CommandLineArgs _instance;

        public List<string> Folders { get; private set; }
        public List<string> TreeFolders { get; private set; }
        public List<string> SearchPatterns { get; private set; }

        private CommandLineArgs(List<string> folders, List<string> treeFolders, List<string> searchPatterns)
        {
            Folders = folders;
            TreeFolders = treeFolders;
            SearchPatterns = searchPatterns;
        }

        public static CommandLineArgs Create(List<string> folders, List<string> treeFolders, List<string> searchPatterns)
        {
            if (_instance == null)
            {
                _instance = new CommandLineArgs(folders, treeFolders, searchPatterns);
            }

            return _instance;
        }
    }
}
