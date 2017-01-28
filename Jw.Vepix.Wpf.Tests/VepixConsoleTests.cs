using Jw.Vepix.Wpf.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jw.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixConsoleTests
    {
        // <MethodNameUnderTest>_<StateUnderTest>_<ExpectedBehavior>()
        // Arrange.
        // Act.
        // Assert.

        VepixConsole _console = VepixConsole.Instance();
        string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string validSearchPattern = "*.jpg";

        [TestMethod]
        public void Parse_HelpSwitch_Valid()
        {
            string[] args = { "-?" };
            Assert.IsFalse(_console.Parse(args));
        }

        [TestMethod]
        public void Parse_TopDirAndSearchPatternSwitches_Valid()
        {
            string[] args = { "-d", _validPath, "-p", validSearchPattern };
            
            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.TopDirectories[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_TopDirSearchPatternAndAllDirSwitches_Valid()
        { 
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] args = { "-d", _validPath, "-a", docsFolder,"-p", validSearchPattern };

            _console.Parse(args);

            Assert.IsTrue(_validPath == _console.TopDirectories[0]
                && docsFolder == _console.AllDirectories[0]
                && validSearchPattern == _console.SearchPatterns[0]);
        }

        [TestMethod]
        public void Parse_2TopDirAnd2SearchPatternSwitches_Valid()
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
        public void Parse_2AllDirAnd2SearchPatternSwitches_Valid()
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
        public void Parse_InvalidTopDirArgument_ThrowsException()
        {
            string[] args = { "-d", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_InvalidAllDirArgument_ThrowsException()
        {
            string[] args = { "-a", "C\\%&^", "-p", validSearchPattern, "1*.png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_InvalidSearchPatternArgument_ThrowsException()
        {
            string[] args = { "-a", _validPath, "-f", validSearchPattern, "1*png" };

            _console.Parse(args);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_InvalidSwitches_ThrowsException()
        {
            string[] args = { "dh", _validPath, "-p", "*.jpg" };

            _console.Parse(args);
        }
    }
}
