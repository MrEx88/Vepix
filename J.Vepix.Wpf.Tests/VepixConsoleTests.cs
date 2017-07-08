using J.Vepix.Wpf.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace J.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixConsoleTests
    {
        // <MethodNameUnderTest>_Should<ExpectedResult> _When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        VepixCommandLineParser _consoleParser = new VepixCommandLineParser();
        VepixCommandLineResults _console = VepixCommandLineParser.ConsoleInstance();
        string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string validSearchPattern = "*.jpg";

        [TestMethod]
        public void Parse_ShouldReturnFalse_WhenHelpSwitchIsUsed()
        {
            string[] args = { "-?" };
            Assert.IsFalse(_consoleParser.Parse(args));
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenTopDirAndPatternSwitchIsUsed()
        {
            string[] args = { "-d", _validPath, "-p", validSearchPattern };
            
            _consoleParser.Parse(args);

            Assert.IsTrue(_validPath == _console.Folders[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenTopDirAllDirAndPatternSwitchIsUsed()
        { 
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] args = { "-d", _validPath, "-a", docsFolder,"-p", validSearchPattern };

            _consoleParser.Parse(args);

            Assert.IsTrue(_validPath == _console.Folders[0]
                && docsFolder == _console.TreeFolders[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2TopDirsAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-d", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _consoleParser.Parse(args);

            Assert.IsTrue(_validPath == _console.Folders[0]
                && "C:\\" == _console.Folders[1]
                && validSearchPattern == _console.SearchPatterns[0]
                && "1*.png" == _console.SearchPatterns[1]
                && _console.TreeFolders.Count == 0);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2AllDirsAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-a", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _consoleParser.Parse(args);

            Assert.IsTrue(_validPath == _console.TreeFolders[0]
                && "C:\\" == _console.TreeFolders[1]
                && validSearchPattern == _console.SearchPatterns[0]
                && "1*.png" == _console.SearchPatterns[1]
                && _console.Folders.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidTopDirSwitchIsUsed()
        {
            string[] args = { "-d", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _consoleParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidAllDirSwitchIsUsed()
        {
            string[] args = { "-a", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _consoleParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidSearchPatternSwitchIsUsed()
        {
            string[] args = { "-a", _validPath, "-f", validSearchPattern, "1*png" };

            _consoleParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowArgumentException_WhenInvalidSwitchesAreUsed()
        {
            string[] args = { "dh", _validPath, "-p", "*.jpg" };

            _consoleParser.Parse(args);
        }
    }
}
