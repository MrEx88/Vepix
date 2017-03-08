using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Jw.Vepix.Wpf.ViewModels;
using Prism.Events;

namespace Jw.Vepix.Wpf.Tests
{
    [TestClass]
    public class VepixWindowViewModelTests
    {

        // <MethodNameUnderTest>_Should<ExpectedResult> _When<Condition>()
        // Arrange.
        // Act.
        // Assert.

        private VepixWindowViewModel _viewModel;
        private Mock<IEventAggregator> _eventAggregator;

        public VepixWindowViewModelTests()
        {
            _eventAggregator = new Mock<IEventAggregator>();
            _viewModel = new VepixWindowViewModel(_eventAggregator.Object);
        }

        [TestMethod]
        public void Constructor_ShouldCallCheckCommandLine_WhenInvoked()
        {

        }
    }
}
