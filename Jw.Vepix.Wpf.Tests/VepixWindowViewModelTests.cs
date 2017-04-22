using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Jw.Vepix.Wpf.ViewModels;
using Prism.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Data;

namespace Jw.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixWindowViewModelTests
    {

        // <MethodNameUnderTest>_Should<ExpectedResult> _When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        private Mock<IEventAggregator> _eventAggregator;
        private Mock<IPictureRepository> _pictureRepo;
        private Mock<VepixWindowViewModel> _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            _eventAggregator = new Mock<IEventAggregator>();
            _pictureRepo = new Mock<IPictureRepository>();
            _viewModel = new Mock<VepixWindowViewModel>( _eventAggregator.Object, _pictureRepo.Object);
        }

        [TestMethod]
        public void Constructor_ShouldCallCheckCommandLine_WhenInvoked()
        {
              
        }
    }
}
