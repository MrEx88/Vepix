using Jw.Vepix.Core.Interfaces;
using Jw.Vepix.Core.Models;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jw.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixWindowViewModelTests
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
        private VepixWindowViewModel _mockVepixWindowviewModel;

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
            _mockVepixWindowviewModel = new VepixWindowViewModel(_mockPictureFolderTreeViewModel.Object,
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
            Assert.IsNull(_mockVepixWindowviewModel.SelectedPictureGridViewModel);
        }

        [TestMethod]
        public void PictureFolderTreeViewModel_ShoulbBeCalledOnce_WhenLoadingPictures()
        {
            string test;
            _mockFileExplorerDialogService.Setup(dialog => dialog.ShowFolderBrowserDialog(out test))
                .Returns(DialogResult.OK);
            _mockPictureRepo.Setup(repo => repo.GetPicturesFromFolderAsync(It.IsAny<string>(), SearchOption.AllDirectories))
                .Returns(It.IsAny<Task<List<Picture>>>());
            _mockVepixWindowviewModel.OpenFolderCommand.Execute(SearchOption.AllDirectories);
            
            _mockPictureFolderTreeViewModel.Verify(vm => vm.Load(It.IsAny<string>()), Times.Once);
        }
    }
}
