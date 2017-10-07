using JW.Vepix.Wpf.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PicturesViewerView.xaml
    /// </summary>
    public partial class PicturesViewerView : UserControl
    {
        private const string GRID_IN_FLIPVIEW = "gridWithTransform";

        private Point? _lastCenterPositionOnTarget;
        private Point? _lastMousePositionOnTarget;
        private double _ratio = 1.0;

        public PicturesViewerView()
        {
            InitializeComponent();
        }

        private void imageBeingViewed_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            var image = ((Image)e.Source).Source as BitmapImage;
            if (image == null)
            {
                return;
            }

            var flipViewWitdth = flipViewImages.ActualWidth;
            var flipViewHeight = flipViewImages.ActualHeight;
            var imageWidth = image.PixelWidth;
            var imageHeight = image.PixelHeight;
            var canvasCustom = ControlHelper.RecurseChildren<CropSelectionCanvas>(flipViewImages).First();

            var ratioWidth = flipViewWitdth / imageWidth;
            var ratioHeight = flipViewHeight / imageHeight;
            _ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            if (_ratio < 1.0)
            {
                sliderZoom.Minimum = _ratio;
                sliderZoom.Value = _ratio;
            }
            else
            {
                _ratio = 1;
                sliderZoom.Minimum = 1;
                sliderZoom.Value = 1;
            }
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (flipViewImages != null)
            {
                var gridTransform = ControlHelper.RecurseChildren<Grid>(flipViewImages)
                                        .First(grid => grid.Name == GRID_IN_FLIPVIEW);
                var transformGroup = (TransformGroup)gridTransform.LayoutTransform;
                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                scaleTransform.ScaleX = e.NewValue * _ratio;
                scaleTransform.ScaleY = e.NewValue * _ratio;

                var scrollViewer = ControlHelper.RecurseChildren<ScrollViewer>(flipViewImages).First();
                var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                _lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, gridTransform);
            }
        }

        private void flipViewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clearViewingState();
        }

        private void flipViewImages_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (flipViewImages.IsVisible == false)
            {
                clearViewingState();
            }
        }

        private void clearViewingState()
        {
            _lastCenterPositionOnTarget = null;

            var canvasCustom = ControlHelper.RecurseChildren<CropSelectionCanvas>(flipViewImages)
                                 .FirstOrDefault(canvas => canvas.IsVisible);
            canvasCustom?.ClearCropShape();

            tglBtnCrop.IsChecked = false;
        }

        private void scrollForImage_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentWidthChange != 0 || e.ExtentHeightChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                //                       Need to filter because FlipView's have several Grid's.
                var gridTransform = ControlHelper.RecurseChildren<Grid>(flipViewImages)
                                        .First(grid => grid.Name == GRID_IN_FLIPVIEW);

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        targetBefore = _lastCenterPositionOnTarget;

                        var scrollViewer = ControlHelper.RecurseChildren<ScrollViewer>(flipViewImages).First();
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);

                        targetNow = scrollViewer.TranslatePoint(centerOfViewport, gridTransform);
                    }
                }
                else
                {
                    targetBefore = _lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(gridTransform);

                    _lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double xTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double yTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double xMultiplier = e.ExtentWidth / gridTransform.ActualWidth;
                    double yMultiplier = e.ExtentHeight / gridTransform.ActualHeight;

                    var scrollViewer = ControlHelper.RecurseChildren<ScrollViewer>(flipViewImages).First();
                    double xNewOffset = scrollViewer.HorizontalOffset - (xTargetPixels * xMultiplier);
                    double yNewOffset = scrollViewer.VerticalOffset - (yTargetPixels * yMultiplier);

                    if (!double.IsNaN(xNewOffset) && !double.IsNaN(yNewOffset))
                    {
                        scrollViewer.ScrollToHorizontalOffset(xNewOffset);
                        scrollViewer.ScrollToVerticalOffset(yNewOffset);
                    }
                }
            }
        }

        private void scrollForImage_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //                     Need to filter because FlipView's have several Grid's.
            var gridTransform = ControlHelper.RecurseChildren<Grid>(flipViewImages)
                                    .First(grid => grid.Name == GRID_IN_FLIPVIEW);
            _lastMousePositionOnTarget = Mouse.GetPosition(gridTransform);

            if (e.Delta > 0)
            {
                sliderZoom.Value += sliderZoom.TickFrequency;
            }

            if (e.Delta < 0)
            {
                sliderZoom.Value -= sliderZoom.TickFrequency;
            }

            e.Handled = true;
        }
    }
}
