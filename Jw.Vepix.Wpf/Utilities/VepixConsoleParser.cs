using Jw.Vepix.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Jw.Vepix.Wpf.Utilities
{
    public class VepixConsoleParser
    {
        public List<string> TopDirectories { get; private set; }
        public List<string> AllDirectories { get; private set; }
        public List<string> SearchPatterns { get; private set; }
        public static string TopDirectoriesHelp =>
            "-d\t\tDirectories. The directories to search in.";
        public static string AllDirectoriesHelp =>
            "-dd\t\tDirectories and Subdirectories to search in.";
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
            .AppendLine(TopDirectoriesHelp)
            .AppendLine(AllDirectoriesHelp)
            .AppendLine(SearchPatternHelp).ToString();

        public static VepixConsole ConsoleInstance()
        {
            if (_vepixConsole == null)
            {
                _vepixConsole = new VepixConsole(new List<string>(), new List<string>(), new List<string>());
            }

            return _vepixConsole;
        }

        //private VepixConsoleParser() {}

        public bool Parse(string[] args)
        {
            var argsList = new List<string>(args);
            if (IsHelpSwitch(argsList))
            {
                DisplayHelp();
                return false;
            }
            else if (!IsValidSwitches(argsList))
            {
                throw new ArgumentException("invalid switch(es) used.");
            }

            argsList.Add(SWITCH_TOKEN);
            var topDirectories = GetArgsOf(TOP_DIRECTORY_SWITCH, argsList, ValidateDirectories);
            var allDirectories = GetArgsOf(ALL_DIRECTORIES_SWITCH, argsList, ValidateDirectories);
            var searchPatterns = GetArgsOf(SEARCH_PATTERN_SWITCH, argsList, ValidateSearchPattern);

            _vepixConsole = new VepixConsole(topDirectories, allDirectories, searchPatterns);

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

        private void DisplayHelp()
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

        private bool ValidateDirectories(List<string> argsList)
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

        private static VepixConsole _vepixConsole;
        private const string SWITCH_TOKEN = "-";
        private const string TOP_DIRECTORY_SWITCH = "-d";
        private const string ALL_DIRECTORIES_SWITCH = "-a";
        private const string SEARCH_PATTERN_SWITCH = "-p";
        private static readonly string REGEX_FILENAME = @"[\w\d\s\-\\_\*]*\.";
        private static readonly List<string> FILE_FORMATS = new List<string>() { "gif", "jpg", "png" };
        private static readonly List<string> HELP_SWITCHES = new List<string>() { "-h", "-?", "-help", "help", "?" };
        private static readonly List<string> SWITCHES = new List<string>()
        {
            TOP_DIRECTORY_SWITCH,
            ALL_DIRECTORIES_SWITCH,
            SEARCH_PATTERN_SWITCH
        };
    }
}
