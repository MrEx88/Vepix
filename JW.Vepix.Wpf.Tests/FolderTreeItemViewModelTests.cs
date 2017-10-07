using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class FolderTreeItemViewModelTests
    {
        private FolderTreeItemViewModel _folderTreeItemViewModel;
        private Mock<IEventAggregator> _mockEventAggregator;
        private string _validPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        [TestInitialize]
        public void Initialize()
        {
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockEventAggregator.Setup(ea => ea.GetEvent<OpenPicturesFromFolderEvent>())
                                .Returns(new OpenPicturesFromFolderEvent());
            _folderTreeItemViewModel = new FolderTreeItemViewModel(new System.IO.DirectoryInfo(_validPath),
                                                                   _mockEventAggregator.Object);
        }
        
        [TestMethod]
        public void Parent_ShouldbeNull_WhenFolderTreeItemViewModelIsContsructed()
        {
            Assert.IsNull(_folderTreeItemViewModel.Parent);
        }

        [TestMethod]
        public void TreeItemAlreadyExists_ShouldReturnTrue_WhenFolderIsAlreadyATreeItem()
        {
            Assert.IsTrue(_folderTreeItemViewModel.TreeItemAlreadyExists(_validPath));
        }

        [TestMethod]
        public void OpenPicturesInFolderCommand_ShouldCallEventAggregatorOnce_WhenCalled()
        {
            _folderTreeItemViewModel.OpenPicturesInFolderCommand.Execute(_folderTreeItemViewModel);
            _mockEventAggregator.Verify(aggregator => aggregator.GetEvent<OpenPicturesFromFolderEvent>(), Times.Once);
        }
    }
}
