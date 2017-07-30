using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        // <[Method/Property]NameUnderTest>_Should<ExpectedResult>_When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        [TestInitialize]
        public void Initialize()
        {
            _mockFolderTreeViewModel = new Mock<IFolderTreeViewModel>();
            _mockFileService = new Mock<IFileService>();
            _mockFileService.Setup(fileService => 
                fileService.GetFileNamesFromDirectoryAsync(It.IsAny<string>(),
                It.IsAny<List<string>>(),It.IsAny<SearchOption>()))
                .Returns(_getFileNamesAsync);
            _mockFileExplorerDialogService = new Mock<IFileExplorerDialogService>();
            _mockPictureGridViewModels = new List<Mock<IPictureGridViewModel>>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockPictureRepo = new Mock<IPictureRepository>();

            var openPicturesFromFolderEvent = new OpenPicturesFromFolderEvent();
            var statusTextUserActionEvent = new StatusTextUserActionEvent();
            var statusTextHelpInfoEvent = new StatusTextHelpInfoEvent();
            _mockEventAggregator.Setup(ea => ea.GetEvent<OpenPicturesFromFolderEvent>())
                .Returns(openPicturesFromFolderEvent);
            _mockEventAggregator.Setup(ea => ea.GetEvent<StatusTextUserActionEvent>())
                .Returns(statusTextUserActionEvent);
            _mockEventAggregator.Setup(ea => ea.GetEvent<StatusTextHelpInfoEvent>())
                .Returns(statusTextHelpInfoEvent);

            _mainViewModel = new MainViewModel(_mockFolderTreeViewModel.Object,
                CreatePictureGridViewModel, _mockFileService.Object, 
                _mockFileExplorerDialogService.Object, _mockEventAggregator.Object);
        }

        private IPictureGridViewModel CreatePictureGridViewModel()
        {
            var mockPictureGridViewModel = new Mock<IPictureGridViewModel>();
            mockPictureGridViewModel.Setup(vm => vm.Load(new List<string> { "C:\\Users\\test.jpg" }));

            _mockPictureGridViewModels.Add(mockPictureGridViewModel);
            return mockPictureGridViewModel.Object;
        }

        [TestMethod]
        public void SelectedGridViewModel_ShouldBeNull_WhenFirstInitialized()
        {
            Assert.IsNull(_mainViewModel.SelectedPictureGridViewModel);
        }

        [TestMethod]
        public void FolderTreeViewModel_ShoulbBeCalledOnce_WhenLoadingPictures()
        {
            string test = "C:\\";
            _mockFileExplorerDialogService.Setup(dialog => dialog.ShowFolderBrowserDialog(out test))
                .Returns(true);
            _mockPictureRepo.Setup(repo => repo.GetPicturesFromFolderAsync(It.IsAny<string>(), SearchOption.AllDirectories))
                .Returns(It.IsAny<Task<List<Picture>>>());

            _mainViewModel.OpenFolderCommand.Execute(SearchOption.AllDirectories);

            _getFileNamesAsync.Wait();
            _mockFolderTreeViewModel.Verify(vm => vm.TryLoad(It.IsAny<string>()), Times.Once);
        }

        private Mock<IFolderTreeViewModel> _mockFolderTreeViewModel;
        private Mock<IFileService> _mockFileService;
        private Mock<IFileExplorerDialogService> _mockFileExplorerDialogService;
        private List<Mock<IPictureGridViewModel>> _mockPictureGridViewModels;
        private Mock<IEventAggregator> _mockEventAggregator;
        private Mock<IPictureRepository> _mockPictureRepo;
        private MainViewModel _mainViewModel;

        // Need to use these on when Command are marked with async and awaiting something.
        private Task<List<string>> _getFileNamesAsync = Task<List<string>>.Factory.StartNew(() =>
        {
            return new List<string>();
        });
    }
}
