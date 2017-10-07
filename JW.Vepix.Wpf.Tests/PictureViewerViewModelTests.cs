using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System.Windows;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class PictureViewerViewModelTests
    {
        private PicturesViewerViewModel _pictureViewerViewModel;
        private Mock<IPictureRepository> _mockPictureRepository;
        private Mock<IMessageDialogService> _mockMessageDialogService;
        private Mock<IEventAggregator> _mockEventAggregator;

        [TestInitialize]
        public void Initialize()
        {
            _mockPictureRepository = new Mock<IPictureRepository>();
            _mockMessageDialogService = new Mock<IMessageDialogService>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockEventAggregator.Setup(ea => ea.GetEvent<CropAreaDrawnEvent>())
                                .Returns(new CropAreaDrawnEvent());
            _mockEventAggregator.Setup(ea => ea.GetEvent<PictureOverwrittenEvent>())
                                .Returns(new PictureOverwrittenEvent());

            _pictureViewerViewModel = new PicturesViewerViewModel(_mockPictureRepository.Object,
                                                                  _mockMessageDialogService.Object,
                                                                  _mockEventAggregator.Object);
        }

        [TestMethod]
        public void CropArea_ShouldBeNull_WhenObjectIsFirstConstructed()
        {
            Assert.IsNull(_pictureViewerViewModel.CropArea);
        }

        [TestMethod]
        public void OnSaveCanExecute_ShouldReturnFalse_WhenCropAreaIsNull()
        {
            Assert.IsFalse(_pictureViewerViewModel.SaveCommand.CanExecute(new object()));
        }

        [TestMethod]
        public void OnSaveCanExecute_ShouldReturnTrue_WhenCropAreaHasAValue()
        {
            _pictureViewerViewModel.CropArea = new Int32Rect(1, 1, 1, 1);

            Assert.IsTrue(_pictureViewerViewModel.SaveCommand.CanExecute(new object()));
        }

        [TestMethod]
        public void Load_ShouldAddPicturesToPicturesObservableCollection_WhenCalled()
        {
            _pictureViewerViewModel.ViewingPicture = new Picture();
            _pictureViewerViewModel.CropArea = new Int32Rect();
            _mockPictureRepository.Setup(repo => repo.GetCroppedImage(new Picture(), new Int32Rect()))
                                  .Returns(It.IsAny<Picture>());
            _mockPictureRepository.Setup(repo => repo.TryOverwrite(new Picture()))
                                  .Returns(It.IsAny<TryResult>());

            _pictureViewerViewModel.SaveCommand.Execute(new object());

            _mockMessageDialogService.Verify(service => service.ShowQuestion(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
