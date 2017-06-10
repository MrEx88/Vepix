using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Results
{
    public class VepixConsoleResults
    {
        public List<string> TopDirectories { get; }
        public List<string> AllDirectories { get; }
        public List<string> SearchPatterns { get; }

        public VepixConsoleResults(List<string> topDirectories, List<string> allDirectories, List<string> searchPatterns)
        {
            TopDirectories = topDirectories;
            AllDirectories = allDirectories;
            SearchPatterns = searchPatterns;
        }
    }
}
