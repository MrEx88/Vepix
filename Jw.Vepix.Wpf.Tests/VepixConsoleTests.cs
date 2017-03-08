using Jw.Vepix.Wpf.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jw.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixConsoleTests
    {
        // <MethodNameUnderTest>_Should<ExpectedResult> _When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        VepixConsole _console = VepixConsole.Instance();
        string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string validSearchPattern = "*.jpg";

        [TestMethod]
        public void Parse_ShouldReturnFalse_WhenHelpSwitchIsUsed()
        {
            string[] args = { "-?" };
            Assert.IsFalse(_console.Parse(args));
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenTopDirAndPatternSwitchIsUsed()
        {
            string[] args = { "-d", _validPath, "-p", validSearchPattern };
            
            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.TopDirectories[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_WhenTopDirAllDirAndPatternSwitchIsUsed()
        { 
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] args = { "-d", _validPath, "-a", docsFolder,"-p", validSearchPattern };

            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.TopDirectories[0]
                && docsFolder == _console.AllDirectories[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2TopDirsAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-d", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.TopDirectories[0]
                && "C:\\" == _console.TopDirectories[1]
                && validSearchPattern == _console.SearchPatterns[0]
                && "1*.png" == _console.SearchPatterns[1]
                && _console.AllDirectories.Count == 0);
        }

        [TestMethod]
        public void Parse_ShouldPopulateListsCorrectly_When2AllDirsAnd1PatternSwitchIsUsed()
        {
            string[] args = { "-a", _validPath, "C:\\", "-p", validSearchPattern, "1*.png" };
            
            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.AllDirectories[0]
                && "C:\\" == _console.AllDirectories[1]
                && validSearchPattern == _console.SearchPatterns[0]
                && "1*.png" == _console.SearchPatterns[1]
                && _console.TopDirectories.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidTopDirSwitchIsUsed()
        {
            string[] args = { "-d", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidAllDirSwitchIsUsed()
        {
            string[] args = { "-a", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowInvalidArgumentException_WhenInvalidSearchPatternSwitchIsUsed()
        {
            string[] args = { "-a", _validPath, "-f", validSearchPattern, "1*png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_ShouldThrowArgumentException_WhenInvalidSwitchesAreUsed()
        {
            string[] args = { "dh", _validPath, "-p", "*.jpg" };

            _console.Parse(args);
        }
    }
}
