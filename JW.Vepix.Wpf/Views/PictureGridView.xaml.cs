using System.Windows.Controls;

namespace JW.Vepix.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PictureGridView.xaml
    /// </summary>
    public partial class PictureGridView : UserControl
    {
        public PictureGridView()
        {
            InitializeComponent();
        }

        private void chkBoxSelectAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chkBoxSelectAll.IsChecked.Value)
            {
                pictureList.SelectAll();
            }
            else
            {
                pictureList.UnselectAll();
            }
        }
    }
}
