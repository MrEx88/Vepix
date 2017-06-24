using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Jw.Vepix.Wpf.Views
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
            if (imageBeingViewed.Source == null)
            {
                return;
            }

            canvasCustom.ClearCropShape();

            var borderWidth = borderViewer.ActualWidth;
            var borderHeight = borderViewer.ActualHeight;
            var imageWidth = (imageBeingViewed.Source as BitmapImage).PixelWidth;
            var imageHeight = (imageBeingViewed.Source as BitmapImage).PixelHeight;

            var ratioWidth = borderWidth / (double)imageWidth;
            var ratioHeight = borderHeight / (double)imageHeight;
            var ratio = ratioWidth > ratioHeight ? ratioWidth : ratioHeight;

            if (ratio < 1.0)
            {
                sliderZoom.Value = ratio;
            }
        }
    }
}
