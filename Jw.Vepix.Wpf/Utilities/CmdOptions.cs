using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Utilities
{
    // CommandLineParser Library does not seem to be working correctly. When help text is generated, nothing shows, and "--help" is displayed twice.
    public class CmdOptions
    {
        [OptionList('d', "TopDirectories", Separator = ',', HelpText = "Directories.The directories to search in.", Required = false)]
        public IList<string> TopDirectories { get; set; }

        [OptionList('a', "AllDirectories", Separator = ',', HelpText = "Directories.The directories to search in.", Required = false)]
        public IList<string> AllDirectories { get; set; }

        [OptionList('f', "FileFilters", Separator = ',', HelpText = "File Filters to use (e.g. \"*.jpg\").", Required = false)]
        public IList<string> FileFilters { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
            => HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));

    }
}
