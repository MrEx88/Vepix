using System.Collections.Generic;

namespace Jw.Vepix.Data
{
    public class VepixConsole
    {
        public List<string> TopDirectories { get; }
        public List<string> AllDirectories { get; }
        public List<string> SearchPatterns { get; }

        public VepixConsole(List<string> topDirectories, List<string> allDirectories, List<string> searchPatterns)
        {
            TopDirectories = topDirectories;
            AllDirectories = allDirectories;
            SearchPatterns = searchPatterns;
        }
    }
}
