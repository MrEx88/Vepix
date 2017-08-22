using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace JW.Vepix.Wpf.Utilities
{
    public class VepixCommandLineParser
    {
        private static VepixCommandLineResults _vepixConsole;
        private const string SWITCH_TOKEN = "-";
        private const string FOLDER_SWITCH = "-f";
        private const string FOLDER_TREE_SWITCH = "-t";
        private const string SEARCH_PATTERN_SWITCH = "-p";
        private const string REGEX_FILENAME = @"[\w\d\s\-\\_\*]*\.";
        private readonly List<string> FILE_FORMATS = new List<string>() { "bmp", "gif", "jpg", "png", "tiff", "wmp" };
        private readonly List<string> HELP_SWITCHES = new List<string>() { "-h", "-?", "-help", "help", "?" };
        private readonly List<string> SWITCHES = new List<string>()
        {
            FOLDER_SWITCH,
            FOLDER_TREE_SWITCH,
            SEARCH_PATTERN_SWITCH
        };

        // todo: Now I know the difference between file filters and search patterns
        // filefilters = "Image Files|*.jpg;*.jpeg;*.png;*.gif" (filter for image file format types)
        // search patterns = "1*.jpg" or "r*.*"
        // for command line, i will change search patterns to allow something like "*.*"
        // when getting the files here, I still need to do the image file filter 
        public List<string> Folders { get; private set; }
        public List<string> TreeFolders { get; private set; }
        public List<string> SearchPatterns { get; private set; }
        public static string FoldersHelp =>
            "-f\t\tFolders. The folders to search in.";
        public static string TreeFoldersHelp =>
            "-t\t\tTree Folders.";
        public static string SearchPatternHelp =>
            "-p\t\tSearch Patterns. The pattern to search with (e.g. \"*.jpg\", \"*2*.png\").";
        public static string CommandLineHelp =>
                new StringBuilder()
            .AppendLine().AppendLine()
            .AppendLine("vepix - Version: ")
            .Append(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion)
            .AppendLine().AppendLine()
            .AppendLine("== Command Line Tool ==")
            .AppendLine("Switches:\tDescription:")
            .AppendLine(FoldersHelp)
            .AppendLine(TreeFoldersHelp)
            .AppendLine(SearchPatternHelp).ToString();

        public static VepixCommandLineResults ResultsInstance()
        {
            if (_vepixConsole == null)
            {
                _vepixConsole = new VepixCommandLineResults(new List<string>(), new List<string>(), new List<string>());
            }

            return _vepixConsole;
        }

        public bool Parse(string[] args)
        {
            var argsList = new List<string>(args);
            if (IsHelpSwitch(argsList))
            {
                return false;
            }
            else if (!IsValidSwitches(argsList))
            {
                throw new ArgumentException("invalid switch(es) used.");
            }

            argsList.Add(SWITCH_TOKEN);
            var folders = GetArgsOf(FOLDER_SWITCH, argsList, ValidateFolders);
            var treeFolders = GetArgsOf(FOLDER_TREE_SWITCH, argsList, ValidateFolders);
            var searchPatterns = GetArgsOf(SEARCH_PATTERN_SWITCH, argsList, ValidateSearchPattern);

            _vepixConsole = new VepixCommandLineResults(folders, treeFolders, searchPatterns);

            return true;
        }

        public static void AttachConsole()
        {
            AttachConsole(-1);
        }

        public static void DetachConsole()
        {
            FreeConsole();
        }

        private bool IsHelpSwitch(List<string> argsList) => HELP_SWITCHES.Contains(argsList[0]);

        public void DisplayHelp()
        {
            AttachConsole(-1);
            Console.WriteLine(CommandLineHelp);
            FreeConsole();
        }

        private bool IsValidSwitches(List<string> argsList)
            => argsList.FindAll(arg => arg.Contains(SWITCH_TOKEN) && arg.Length == 2)
                .TrueForAll(arg => SWITCHES.Exists(vSwitch => vSwitch == arg));

        private List<string> GetArgsOf(string vepixSwitch, List<string> argsList, 
            Func<List<string>, bool> ValidationRule)
        {
            int start = argsList.FindIndex(l => l == vepixSwitch) + 1;
            int offset = argsList.FindIndex(start, l => l.Contains(SWITCH_TOKEN));
            var switchArgs = argsList.GetRange(start, offset - start);

            if (!ValidationRule(switchArgs))
            {
                throw new ArgumentException(
                    string.Format("Invalid argument has been used for the {0} switch.", 
                                  vepixSwitch));
            }

            return switchArgs;
        }

        private bool ValidateFolders(List<string> argsList)
            => argsList.Count == 0 ? true
                : argsList.TrueForAll(folder => Directory.Exists(folder));

        private bool ValidateSearchPattern(List<string> argsList)
        {
            if (argsList.Count == 0)
            {
                return true;
            }

            var regularExpression = new StringBuilder().Append(REGEX_FILENAME).Append("(");
            FILE_FORMATS.ForEach(fileFormat => 
                regularExpression.Append(string.Format("{0}|", fileFormat)));
            regularExpression = regularExpression.Remove(regularExpression.Length - 1, 1)
                                                 .Append(")");

            return argsList.TrueForAll(fileFilter => 
                Regex.IsMatch(fileFilter, regularExpression.ToString()));
        }

        #region Interop methods
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        #endregion
    }
}
