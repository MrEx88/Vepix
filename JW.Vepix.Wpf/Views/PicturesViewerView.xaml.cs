using JW.Vepix.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JW.Vepix.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PicturesViewerView.xaml
    /// </summary>
    public partial class PicturesViewerView : UserControl
    {
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

            var borderWidth = borderViewer.ActualWidth;
            var borderHeight = borderViewer.ActualHeight;
            var imageWidth = (((Image)e.Source).Source as BitmapImage).PixelWidth;
            var imageHeight = (((Image)e.Source).Source as BitmapImage).PixelHeight;

            var ratioWidth = borderWidth / (double)imageWidth;
            var ratioHeight = borderHeight / (double)imageHeight;
            var ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            if (ratio < 1.0)
            {
                sliderZoom.Value = ratio;
                sliderZoom.Minimum = ratio;
            }
            else
            {
                sliderZoom.Minimum = 1;
            }
        }

        private void sliderZoom_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (flipViewImages?.SelectedItem == null || flipViewImages?.IsVisible == false)
            {
                return;
            }

            //var grid = RecurseChildren<Grid>(flipViewImages)
            //                     .First(g => g.Name == "adfk");
            //var scaleTransform1 = grid.LayoutTransform as TransformGroup;
            //var scaleTransform = scaleTransform1.Children[0] as ScaleTransform;
            var scaleTransform = RecurseChildren<CropSelectionCanvas>(flipViewImages)
                                 .FirstOrDefault(canvas => canvas.IsVisible).LayoutTransform as ScaleTransform;
            scaleTransform.ScaleX = e.NewValue;
            scaleTransform.ScaleY = e.NewValue;

            var scrollViewer = RecurseChildren<ScrollViewer>(flipViewImages).First(scroll => scroll.IsVisible);
            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            var canvasCustom = RecurseChildren<CropSelectionCanvas>(flipViewImages)
                                 .FirstOrDefault(canvas => canvas.IsVisible);
            _lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, canvasCustom);
        }

        private void imageBeingViewed_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            //cheese
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

        private void flipViewImages_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
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

                if (_lastCenterPositionOnTarget.HasValue)
                {
                    var grid = RecurseChildren<Grid>(flipViewImages)
                                 .First();
                    var canvasCustom = RecurseChildren<CropSelectionCanvas>(flipViewImages)
                                 .FirstOrDefault(canvas => canvas.IsVisible);
                    var centerOfViewport = new Point(((ScrollViewer)sender).ViewportWidth / 2, ((ScrollViewer)sender).ViewportHeight / 2);
                    Point centerOfTargetNow = ((ScrollViewer)sender).TranslatePoint(centerOfViewport, canvasCustom);

                    targetBefore = _lastCenterPositionOnTarget;
                    targetNow = centerOfTargetNow;
                }
                
                if (targetBefore.HasValue)
                {
                    var leftTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    var topTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    var xMultiplicator = ((ScrollViewer)sender).ExtentWidth / flipViewImages.Width;
                    var yMultiplicator = ((ScrollViewer)sender).ExtentHeight / flipViewImages.Height;

                    var xNewOffset = ((ScrollViewer)sender).HorizontalOffset - leftTargetPixels * xMultiplicator;
                    var yNewOffset = ((ScrollViewer)sender).VerticalOffset - topTargetPixels * yMultiplicator;
                    
                    if (double.IsNaN(xNewOffset) || double.IsNaN(yNewOffset))
                    {
                        return;
                    }

                    ((ScrollViewer)sender).ScrollToHorizontalOffset(xNewOffset);
                    ((ScrollViewer)sender).ScrollToVerticalOffset(yNewOffset);
                }
            }
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

        private Point? _lastCenterPositionOnTarget;
    }
}
