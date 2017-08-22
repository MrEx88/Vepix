using JW.Vepix.Wpf.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixCommandLineTests
    {
        private VepixCommandLineParser _consoleParser = new VepixCommandLineParser();
        private string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private string validSearchPattern = "*.jpg";

        // <MethodNameUnderTest>_Should<ExpectedResult>_When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        [TestMethod]
        public void Parse_ShouldReturnFalse_WhenHelpSwitchIsUsed()
        {
            string[] args = { "-?" };
            Assert.IsFalse(_consoleParser.Parse(args));
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenTopDirAndPatternSwitchIsUsed()
        {
            string[] args = { "-f", _validPath, "-p", validSearchPattern };
            
            _consoleParser.Parse(args);
            var results = VepixCommandLineParser.ResultsInstance();
            Assert.IsTrue(_validPath == results.Folders[0]
                && validSearchPattern == results.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenFolderFolderTreeAndPatternSwitchIsUsed()
        { 
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] args = { "-f", _validPath, "-t", docsFolder,"-p", validSearchPattern };

            _consoleParser.Parse(args);
            var results = VepixCommandLineParser.ResultsInstance();
            Assert.IsTrue(_validPath == results.Folders[0]
                && docsFolder == results.TreeFolders[0]
                && validSearchPattern == results.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2FoldersAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-f", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _consoleParser.Parse(args);
            var results = VepixCommandLineParser.ResultsInstance();
            Assert.IsTrue(_validPath == results.Folders[0]
                && "C:\\" == results.Folders[1]
                && validSearchPattern == results.SearchPatterns[0]
                && "1*.png" == results.SearchPatterns[1]
                && results.TreeFolders.Count == 0);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2FolderTreesAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-t", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _consoleParser.Parse(args);
            var results = VepixCommandLineParser.ResultsInstance();
            Assert.IsTrue(_validPath == results.TreeFolders[0]
                && "C:\\" == results.TreeFolders[1]
                && validSearchPattern == results.SearchPatterns[0]
                && "1*.png" == results.SearchPatterns[1]
                && results.Folders.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidTopDirSwitchIsUsed()
        {
            string[] args = { "-f", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _consoleParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidAllDirSwitchIsUsed()
        {
            string[] args = { "-t", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _consoleParser.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidSearchPatternSwitchIsUsed()
        {
            string[] args = { "-t", _validPath, "-f", validSearchPattern, "1*pg" };

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
