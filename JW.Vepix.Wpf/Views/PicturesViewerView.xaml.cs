using JW.Vepix.Wpf.Controls;
using System.Collections.Generic;
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
        private double _ratio = 1.0;
        private Point? _lastCenterPositionOnTarget;
        private Point? _lastMousePositionOnTarget;
        private Point? _lastDragPoint;

        public PicturesViewerView()
        {
            InitializeComponent();
        }

        private void imageBeingViewed_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (((Image)e.Source).Source == null)
            {
                return;
            }

            var flipViewWitdth = flipViewImages.ActualWidth;
            var flipViewHeight = flipViewImages.ActualHeight;
            var imageWidth = (((Image)e.Source).Source as BitmapImage).PixelWidth;
            var imageHeight = (((Image)e.Source).Source as BitmapImage).PixelHeight;

            var ratioWidth = flipViewWitdth / (double)imageWidth;
            var ratioHeight = flipViewHeight / (double)imageHeight;
            _ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            if (_ratio < 1.0)
            {
                sliderZoom.Minimum = _ratio;
                sliderZoom.Value = _ratio;
                var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == GRID_IN_FLIPVIEW);
                var transformGroup = gridTransform.LayoutTransform as TransformGroup;
                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                scaleTransform.ScaleX *= _ratio;
                scaleTransform.ScaleY *= _ratio;
            }
            else
            {
                sliderZoom.Minimum = 1;
            }
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (flipViewImages != null)
            {
                var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == GRID_IN_FLIPVIEW);
                var transformGroup = (TransformGroup)gridTransform.LayoutTransform;
                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                scaleTransform.ScaleX = e.NewValue * _ratio;
                scaleTransform.ScaleY = e.NewValue * _ratio;

                var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
                var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                _lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, gridTransform);
            }
        }

        private void flipViewImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _lastCenterPositionOnTarget = null;

            var canvasCustom = RecurseChildren<CropSelectionCanvas>(flipViewImages)
                                 .FirstOrDefault(canvas => canvas.IsVisible);
            canvasCustom?.ClearCropShape();
        }              

        private void scrollForImage_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentWidthChange != 0 || e.ExtentHeightChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                //                                                         Need to filter because FlipView's have several Grid's.
                var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == GRID_IN_FLIPVIEW);

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        targetBefore = _lastCenterPositionOnTarget;

                        var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
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

                    var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
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

        //private void scrollForImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (_lastDragPoint.HasValue)
        //    {
        //        var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
        //        var positionNow = e.GetPosition(scrollViewer);

        //        var x = positionNow.X - _lastDragPoint.Value.X;
        //        var y = positionNow.Y - _lastDragPoint.Value.Y;

        //        _lastDragPoint = positionNow;

        //        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - x);
        //        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - y);
        //    }
        //}

        //private void scrollForImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
        //    scrollViewer.Cursor = Cursors.Arrow;
        //    scrollViewer.ReleaseMouseCapture();
        //    _lastDragPoint = null;
        //}

        //private void scrollForImage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
        //    var mousePosition = e.GetPosition(scrollViewer);
        //    if (mousePosition.X <= scrollViewer.ViewportWidth
        //        && mousePosition.Y < scrollViewer.ViewportHeight) // Makes sure we can still use the scrollbars.
        //    {
        //        scrollViewer.Cursor = Cursors.SizeAll;
        //        _lastDragPoint = mousePosition;
        //        Mouse.Capture(scrollViewer);
        //    }
        //}

        private void scrollForImage_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //                                                         Need to filter because FlipView's have several Grid's.
            var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == GRID_IN_FLIPVIEW);
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

        //todo: find some where to put this
        public static IEnumerable<T> RecurseChildren<T>(DependencyObject root) where T : UIElement
        {
            if (root is T)
            {
                yield return root as T;
            }

            if (root != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(root);

                for (var index = 0; index < count; index++)
                {
                    foreach (var child in RecurseChildren<T>(VisualTreeHelper.GetChild(root, index)))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
