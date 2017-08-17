using System;
using System.ComponentModel;
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

            ((INotifyPropertyChanged)pictureList.Items).PropertyChanged += PictureGridView_PropertyChanged;
        }

        private void PictureGridView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Count")
            {
                setAllSelectedState(chkBoxSelectAll.IsChecked.Value);
            }
        }

        private void chkBoxSelectAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setAllSelectedState(chkBoxSelectAll.IsChecked.Value);
        }

        private void setAllSelectedState(bool isSelectAllCheck)
        {
            var action = isSelectAllCheck ? (Action)pictureList.SelectAll : pictureList.UnselectAll;
            action();
        }
    }
}
