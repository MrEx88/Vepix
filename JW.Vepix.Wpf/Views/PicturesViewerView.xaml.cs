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

            //var borderWidth = borderViewer.ActualWidth;
            //var borderHeight = borderViewer.ActualHeight;
            //var imageWidth = (((Image)e.Source).Source as BitmapImage).PixelWidth;
            //var imageHeight = (((Image)e.Source).Source as BitmapImage).PixelHeight;

            //var ratioWidth = borderWidth / (double)imageWidth;
            //var ratioHeight = borderHeight / (double)imageHeight;
            //var ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            //if (ratio < 1.0)
            //{
            //    sliderZoom.Value = ratio;
            //    sliderZoom.Minimum = ratio;
            //}
            //else
            //{
            //    sliderZoom.Minimum = 1;
            //}
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (flipViewImages != null)
            {
                var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == GRID_IN_FLIPVIEW);
                var transformGroup = gridTransform.LayoutTransform as TransformGroup;
                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                scaleTransform.ScaleX = e.NewValue;
                scaleTransform.ScaleY = e.NewValue;

                var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
                var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                _lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, gridTransform);
            }
        }

        private void imageBeingViewed_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //scrollForImage.
            //var borderWidth = borderViewer.ActualWidth;
            //var borderHeight = borderViewer.ActualHeight;
            //var imageWidth = (imageBeingViewed.Source as BitmapImage).PixelWidth;
            //var imageHeight = (imageBeingViewed.Source as BitmapImage).PixelHeight;

            //var ratioWidth = borderWidth / (double)imageWidth;
            //var ratioHeight = borderHeight / (double)imageHeight;
            //var ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            //if (ratio < 1.0)
            //{
            //    //scrollForImage.Width = scrollForImage.ScrollableWidth * ratio;
            //    //scrollForImage.Height = scrollForImage.ScrollableHeight * ratio;
            //}
        }

        private void flipViewImages_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

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

                //                                                         Need to filter because FlipView's have several Grid's
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
                    targetNow = Mouse.GetPosition(flipViewImages);

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

        private void scrollForImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_lastDragPoint.HasValue)
            {
                var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
                var positionNow = e.GetPosition(scrollViewer);

                var x = positionNow.X - _lastDragPoint.Value.X;
                var y = positionNow.Y - _lastDragPoint.Value.Y;

                _lastDragPoint = positionNow;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - x);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - y);
            }
        }

        private void scrollForImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            _lastDragPoint = null;
        }

        private void scrollForImage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First();
            var mousePosition = e.GetPosition(scrollViewer);
            if (mousePosition.X <= scrollViewer.ViewportWidth
                && mousePosition.Y < scrollViewer.ViewportHeight) // Makes sure we can still use the scrollbars
            {
                scrollViewer.Cursor = Cursors.SizeAll;
                _lastDragPoint = mousePosition;
                Mouse.Capture(scrollViewer);
            }
        }

        private void scrollForImage_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //                                                         Need to filter because FlipView's have several Grid's
            var gridTransform = RecurseChildren<Grid>(flipViewImages).First(grid => grid.Name == "gridTransform");
            _lastMousePositionOnTarget = Mouse.GetPosition(gridTransform);

            if (e.Delta > 0)
            {
                sliderZoom.Value += 1;
            }

            if (e.Delta < 0)
            {
                sliderZoom.Value -= 1;
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

                for (var idx = 0; idx < count; idx++)
                {
                    foreach (var child in RecurseChildren<T>(VisualTreeHelper.GetChild(root, idx)))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
