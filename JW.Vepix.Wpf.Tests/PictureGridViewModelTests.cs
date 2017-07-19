using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class PictureGridViewModelTests
    {
        // <MethodNameUnderTest>_Should<ExpectedResult>_When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        [TestInitialize]
        public void Initialize()
        {
            _mockPictureRepository = new Mock<IPictureRepository>();
            var onPictureOverwrittenEvent = new PictureOverwrittenEvent();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockEventAggregator.Setup(ea => ea.GetEvent<PictureOverwrittenEvent>())
                .Returns(onPictureOverwrittenEvent);
            _mockModalDialog = new Mock<IMessageDialogService>();
            _mockFileExplorerDialogService = new Mock<IFileExplorerDialogService>();

            _pictureGridViewModel = new PictureGridViewModel(_mockPictureRepository.Object,
                                                             _mockEventAggregator.Object,
                                                             _mockModalDialog.Object,
                                                             _mockFileExplorerDialogService.Object);
        }

        [TestMethod]
        public void Load_ShouldSetFolderName_WhenCalled()
        {
            _mockPictureRepository.Setup(repo => repo.GetPicturesAsync(It.IsAny<string[]>()))
                .Returns(It.IsAny<Task<List<Picture>>>());

            _pictureGridViewModel.Load(new List<string>() { "C:\\test.txt" });
            
            Assert.IsFalse(string.IsNullOrWhiteSpace(_pictureGridViewModel.FolderName));
        }

        [TestMethod]
        public void Load_ShouldSetPicturesLoadingToFalse_WhenTaskIsFinished()
        {
            var task = Task<List<Picture>>.Factory.StartNew(() => new List<Picture>());
            _mockPictureRepository.Setup(repo => repo.GetPicturesAsync(It.IsAny<string[]>()))
                .Returns(It.IsAny<Task<List<Picture>>>());

            _pictureGridViewModel.Load(new List<string>() { "C:\\test.txt" });

            task.Wait();
            Assert.IsFalse(_pictureGridViewModel.ArePicturesLoading);
        }

        [TestMethod]
        public void ClosePictureCommand_ShouldRemoveAPicture_WhenAskingToRemoveAPicture()
        {
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            var beforeCount = _pictureGridViewModel.Pictures.Count;

            _pictureGridViewModel.ClosePictureCommand.Execute(It.IsAny<Picture>());
            var afterCount = _pictureGridViewModel.Pictures.Count;

            Assert.IsTrue(beforeCount > afterCount);
        }

        [TestMethod]
        public void ClosesPictureCommand_ShouldRemove2Pictures_WhenAskingToRemove2Pictures()
        {
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            var beforeCount = _pictureGridViewModel.Pictures.Count;

            _pictureGridViewModel.ClosePicturesCommand.Execute(new List<Picture>() { It.IsAny<Picture>(), It.IsAny<Picture>() });
            var afterCount = _pictureGridViewModel.Pictures.Count;

            Assert.IsTrue(beforeCount - afterCount == 2);
        }

        private PictureGridViewModel _pictureGridViewModel;
        private Mock<IPictureRepository> _mockPictureRepository;
        private Mock<IEventAggregator> _mockEventAggregator;
        private Mock<IMessageDialogService> _mockModalDialog;
        private Mock<IFileExplorerDialogService> _mockFileExplorerDialogService;
    }
}
