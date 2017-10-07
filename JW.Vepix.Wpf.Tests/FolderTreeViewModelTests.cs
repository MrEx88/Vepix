using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class FolderTreeViewModelTests
    {
        private FolderTreeViewModel _folderTreeViewModel;
        private Mock<IEventAggregator> _mockEventAggregator;
        private string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        [TestInitialize]
        public void Initialize()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _folderTreeViewModel = new FolderTreeViewModel(_mockEventAggregator.Object);
        }

        [TestMethod]
        public void TryLoad_ShouldReturnTrue_WhenFolderDoesntExistInTree()
        {
            var expected = _folderTreeViewModel.TryLoad(_validPath);

            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TryLoad_ShouldReturnFalse_WhenFolderAlreadyExistsInTree()
        {
            _folderTreeViewModel.TryLoad(_validPath);
            var expected = _folderTreeViewModel.TryLoad(_validPath);

            Assert.IsFalse(expected);
        }

        [TestMethod]
        public void TryLoad_ShouldAddFolderToFolderTreeItemViewModel_WhenItDoesntExistInTree()
        {
            _folderTreeViewModel.TryLoad(_validPath);

            Assert.AreEqual(_validPath, _folderTreeViewModel.FolderItemViewModels[0].AbsolutePath);
        }

        [TestMethod]
        public void TryLoad_ShouldNotAddAnotherFolderToFolderTreeItemViewModel_WhenItDoesExistInTree()
        {
            _folderTreeViewModel.TryLoad(_validPath);
            _folderTreeViewModel.TryLoad(_validPath);

            Assert.IsTrue(_folderTreeViewModel.FolderItemViewModels.Count == 1);
        }
    }
}
