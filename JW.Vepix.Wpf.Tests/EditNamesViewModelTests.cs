using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JW.Vepix.Wpf.Tests
{
    [TestClass]
    public class EditNamesViewModelTests
    {
        [TestInitialize]
        public void Initialize()
        {
            _mockPictureRepository = new Mock<IPictureRepository>();
            _mockMessageDiaglogService = new Mock<IMessageDialogService>();
            _mockEventAggregator = new Mock<IEventAggregator>();

            _editNamesViewModel = new EditNamesViewModel(_mockPictureRepository.Object,
                                                         _mockMessageDiaglogService.Object,
                                                         _mockEventAggregator.Object);
        }

        [TestMethod]
        public void RemovePictureCommand_ShouldRemove1EditPictureNameAtATime_WhenCalledOnce()
        {
            var editPictureNames = new List<AffixedPictureName> {
                It.IsAny<AffixedPictureName>(), It.IsAny<AffixedPictureName>() };
            _editNamesViewModel.EditPictureNames = new ObservableCollection<AffixedPictureName>(editPictureNames);
            
            _editNamesViewModel.RemovePictureCommand.Execute(0);

            Assert.IsTrue(_editNamesViewModel.EditPictureNames.Count + 1 == editPictureNames.Count);
        }

        [TestMethod]
        public void IsAllPrefixChecked_ShouldHaveAllPrefixesOn_WhenChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            editPictureNames.ToList().ForEach(edit =>  _editNamesViewModel.EditPictureNames.Add(edit.Object));
            _editNamesViewModel.IsAllPrefixChecked = true;

            Assert.IsTrue(editPictureNames.All(edit => edit.Object.IsPrefixOn));
        }

        [TestMethod]
        public void IsAllPrefixesCheckedCommand_ShouldCheckCheckAll_WhenAllAreChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            editPictureNames.ToList().ForEach(edit =>
            {
                edit.Object.IsPrefixOn = true;
                _editNamesViewModel.EditPictureNames.Add(edit.Object);
            });

            _editNamesViewModel.IsAllPrefixesCheckedCommand.Execute(null);

            Assert.IsTrue(_editNamesViewModel.IsAllPrefixChecked);
        }

        [TestMethod]
        public void IsAllPrefixesCheckedCommand_ShouldNotCheckCheckAll_WhenAllAreNotChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            var first = editPictureNames.ToList().First();
            first.Object.IsPrefixOn = true;
            editPictureNames.ToList().ForEach(edit =>
            {
                _editNamesViewModel.EditPictureNames.Add(edit.Object);
            });

            _editNamesViewModel.IsAllPrefixesCheckedCommand.Execute(null);

            Assert.IsFalse(_editNamesViewModel.IsAllPrefixChecked);
        }

        [TestMethod]
        public void IsAllSuffixChecked_ShouldHaveAllPrefixesOn_WhenChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            editPictureNames.ToList().ForEach(edit => _editNamesViewModel.EditPictureNames.Add(edit.Object));
            _editNamesViewModel.IsAllSuffixChecked = true;

            Assert.IsTrue(editPictureNames.All(edit => edit.Object.IsSuffixOn));
        }

        [TestMethod]
        public void IsAllSuffixesCheckedCommand_ShouldCheckCheckAll_WhenAllAreChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            editPictureNames.ToList().ForEach(edit =>
            {
                edit.Object.IsSuffixOn = true;
                _editNamesViewModel.EditPictureNames.Add(edit.Object);
            });

            _editNamesViewModel.IsAllSuffixesCheckedCommand.Execute(null);

            Assert.IsTrue(_editNamesViewModel.IsAllSuffixChecked);
        }

        [TestMethod]
        public void IsAllSuffixesCheckedCommand_ShouldNotCheckCheckAll_WhenAllAreNotChecked()
        {
            var editPictureNames = new List<Mock<AffixedPictureName>> {
                new Mock<AffixedPictureName>(new Picture()), new Mock<AffixedPictureName>(new Picture()) };
            var first = editPictureNames.ToList().First();
            first.Object.IsSuffixOn = true;
            editPictureNames.ToList().ForEach(edit =>
            {
                _editNamesViewModel.EditPictureNames.Add(edit.Object);
            });

            _editNamesViewModel.IsAllSuffixesCheckedCommand.Execute(null);

            Assert.IsFalse(_editNamesViewModel.IsAllSuffixChecked);
        }

        private EditNamesViewModel _editNamesViewModel;
        private Mock<IPictureRepository> _mockPictureRepository;
        private Mock<IMessageDialogService> _mockMessageDiaglogService;
        private Mock<IEventAggregator> _mockEventAggregator;
    }
}
