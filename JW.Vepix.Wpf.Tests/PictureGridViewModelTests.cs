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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
            
            Assert.IsFalse(string.IsNullOrWhiteSpace(_pictureGridViewModel.ViewTitle));
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
        public void ClosePicturesCommand_ShouldRemove2Pictures_WhenAskingToRemove2Pictures()
        {
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            var beforeCount = _pictureGridViewModel.Pictures.Count;

            _pictureGridViewModel.ClosePicturesCommand.Execute(_pictureGridViewModel.Pictures.ToList());
            var afterCount = _pictureGridViewModel.Pictures.Count;

            Assert.IsTrue(beforeCount - afterCount == 2);
        }

        [TestMethod]
        public void EditPictureNameCommand_ShouldChangeName_WhenNameIsValid()
        {
            var task = Task<string>.Factory.StartNew(() => It.IsAny<string>());
            _mockModalDialog.Setup(diaog => diaog.ShowInput(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns( Task<string>.Factory.StartNew(() => "C:\\test2.jpg"));
            var tryResult = new TryResult();
            tryResult.Try(() => { });
            _mockPictureRepository.Setup(repo => repo.TryChangePictureName(It.IsAny<Picture>(), It.IsAny<string>()))
                .Returns(tryResult);
            _pictureGridViewModel.Pictures.Add(new Picture(new BitmapImage(), string.Empty));
            var testFileName = "C:\\test.jpg";
            _pictureGridViewModel.Pictures.First().FullFileName = testFileName;

            _pictureGridViewModel.EditPictureNameCommand.Execute(_pictureGridViewModel.Pictures.First());

            task.Wait();
            Assert.AreNotEqual(testFileName, _pictureGridViewModel.Pictures.First().FullFileName);
        }

        [TestMethod]
        public void EditPictureNameCommand_ShouldNotChangeName_WhenNameIsInvalid()
        {
            var task = Task<string>.Factory.StartNew(() => It.IsAny<string>());
            _mockModalDialog.Setup(diaog => diaog.ShowInput(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<string>.Factory.StartNew(() => "C:\\test2.jpg"));
            var tryResult = new TryResult();
            tryResult.Try(() => { throw new System.Exception(); });
            _mockPictureRepository.Setup(repo => repo.TryChangePictureName(It.IsAny<Picture>(), It.IsAny<string>()))
                .Returns(tryResult);
            _pictureGridViewModel.Pictures.Add(new Picture(new BitmapImage(), string.Empty));
            var testFileName = "C:\\test.jpg";
            _pictureGridViewModel.Pictures.First().FullFileName = "C:\\test.jpg";

            _pictureGridViewModel.EditPictureNameCommand.Execute(_pictureGridViewModel.Pictures.First());

            task.Wait();
            Assert.AreEqual(testFileName, _pictureGridViewModel.Pictures.First().FullFileName);
        }

        [TestMethod]
        public void EditSelectedPictureNamesCommand_ShouldOpenModalDialog_WhenThereIsOnly1PictureSelected()
        {
            var task = Task<string>.Factory.StartNew(() => It.IsAny<string>());
            _mockModalDialog.Setup(diaog => diaog.ShowInput(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(It.IsAny<Task<string>>());
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());
            _pictureGridViewModel.Pictures.Add(It.IsAny<Picture>());

            _pictureGridViewModel.EditSelectedPictureNamesCommand.Execute(new List<Picture>() { new Picture(new BitmapImage(), string.Empty) });

            task.Wait();
            _mockModalDialog.Verify(dialog => dialog.ShowInput(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void DeletePicturesCommand_ShouldRemovePicture_WhenYesIsPressed()
        {
            _mockModalDialog.Setup(dialog => dialog.ShowQuestion(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var tryResult = new TryResult();
            tryResult.Try(() => { });
            _mockPictureRepository.Setup(repo => repo.TryDelete(It.IsAny<string>()))
                .Returns(tryResult);
            _pictureGridViewModel.Pictures.Add(new Picture(new BitmapImage(), string.Empty));
            var beforeRemoveCount = _pictureGridViewModel.Pictures.Count;

            _pictureGridViewModel.DeletePicturesCommand.Execute(_pictureGridViewModel.Pictures.ToList());

            Assert.AreNotEqual(beforeRemoveCount, _pictureGridViewModel.Pictures.Count);
        }

        [TestMethod]
        public void DeletePicturesCommand_ShouldOnlyRemoveSelectedPictures_WhenYesIsPressed()
        {
            _mockModalDialog.Setup(dialog => dialog.ShowQuestion(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var tryResult = new TryResult();
            tryResult.Try(() => { });
            _mockPictureRepository.Setup(repo => repo.TryDelete(It.IsAny<string>()))
                .Returns(tryResult);
            var picturesExist = new Dictionary<Guid, bool>();
            for (var i = 0; i < 3; i++)
            {
                var picture = new Picture(new BitmapImage(), string.Empty);
                _pictureGridViewModel.Pictures.Add(picture);
                // All odd numbers should not exist.
                picturesExist.Add(picture.Guid, !(i % 2 == 0));
            }
            var picturesToRemove = _pictureGridViewModel.Pictures.ToList().FindAll(pic =>
            {
                var picturesThatDontExist = picturesExist.Where(picExist => picExist.Value == false)
                    .ToDictionary(key => key.Key);
                return picturesThatDontExist.ContainsKey(pic.Guid);
            });

            _pictureGridViewModel.DeletePicturesCommand.Execute(picturesToRemove);

            Assert.IsTrue(picturesExist.All(pic => 
            {
                return pic.Value 
                    ? _pictureGridViewModel.Pictures.SingleOrDefault(oPic => oPic.Guid == pic.Key) != null
                    : _pictureGridViewModel.Pictures.SingleOrDefault(oPic => oPic.Guid == pic.Key) == null;
            }));
        }

        [TestMethod]
        public void DeletePicturesCommand_ShouldNotRemovePicture_WhenNoIsPressed()
        {
            _mockModalDialog.Setup(dialog => dialog.ShowQuestion(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            _pictureGridViewModel.Pictures.Add(new Picture(new BitmapImage(), string.Empty));
            var beforeRemoveCount = _pictureGridViewModel.Pictures.Count;

            _pictureGridViewModel.DeletePicturesCommand.Execute(_pictureGridViewModel.Pictures.ToList());

            Assert.AreEqual(beforeRemoveCount, _pictureGridViewModel.Pictures.Count);
        }

        private PictureGridViewModel _pictureGridViewModel;
        private Mock<IPictureRepository> _mockPictureRepository;
        private Mock<IEventAggregator> _mockEventAggregator;
        private Mock<IMessageDialogService> _mockModalDialog;
        private Mock<IFileExplorerDialogService> _mockFileExplorerDialogService;
    }
}
