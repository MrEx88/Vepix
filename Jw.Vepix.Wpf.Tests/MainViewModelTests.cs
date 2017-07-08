using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
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

        // <[Method/Property]NameUnderTest>_Should<ExpectedResult> _When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        private Mock<IPictureFolderTreeViewModel> _mockPictureFolderTreeViewModel;
        private Mock<IFileExplorerDialogService> _mockFileExplorerDialogService;
        private List<Mock<IPictureGridViewModel>> _mockPictureGridViewModels;
        private Mock<IEventAggregator> _mockEventAggregator;
        private OpenPicturesFromFolderEvent _openPicturesFromFolderEvent;
        private Mock<IPictureRepository> _mockPictureRepo;
        private MainViewModel _mockMainViewModel;

        [TestInitialize]
        public void Initialize()
        {
            _mockPictureFolderTreeViewModel = new Mock<IPictureFolderTreeViewModel>();
            _mockFileExplorerDialogService = new Mock<IFileExplorerDialogService>();
            _mockPictureGridViewModels = new List<Mock<IPictureGridViewModel>>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _openPicturesFromFolderEvent = new OpenPicturesFromFolderEvent();
            _mockEventAggregator.Setup(ea => ea.GetEvent<OpenPicturesFromFolderEvent>()).
                Returns(_openPicturesFromFolderEvent);
             _mockPictureRepo = new Mock<IPictureRepository>();
            _mockMainViewModel = new MainViewModel(_mockPictureFolderTreeViewModel.Object,
                CreatePictureGridViewModel, _mockFileExplorerDialogService.Object, _mockEventAggregator.Object);
        }

        private IPictureGridViewModel CreatePictureGridViewModel()
        {
            var mockPictureGridViewModel = new Mock<IPictureGridViewModel>();
            mockPictureGridViewModel.Setup(vm => vm.Load(It.IsAny<List<string>>()))
                .Callback<List<Picture>>(pictures =>
                {
                    mockPictureGridViewModel.Setup(vm => vm.FolderName)
                    .Returns(pictures.FirstOrDefault().FolderName);
                });

            _mockPictureGridViewModels.Add(mockPictureGridViewModel);
            return mockPictureGridViewModel.Object;
        }

        [TestMethod]
        public void SelectedGridViewModel_ShouldBeNull_WhenFirstInitialized()
        {
            Assert.IsNull(_mockMainViewModel.SelectedPictureGridViewModel);
        }

        [TestMethod]
        public void PictureFolderTreeViewModel_ShoulbBeCalledOnce_WhenLoadingPictures()
        {
            string test;
            _mockFileExplorerDialogService.Setup(dialog => dialog.ShowFolderBrowserDialog(out test))
                .Returns(DialogResult.OK);
            _mockPictureRepo.Setup(repo => repo.GetPicturesFromFolderAsync(It.IsAny<string>(), SearchOption.AllDirectories))
                .Returns(It.IsAny<Task<List<Picture>>>());
            _mockMainViewModel.OpenFolderCommand.Execute(SearchOption.AllDirectories);
            
            //_mockPictureFolderTreeViewModel.Verify(vm => vm.Load(It.IsAny<string>()), Times.Once);
        }
    }
}
