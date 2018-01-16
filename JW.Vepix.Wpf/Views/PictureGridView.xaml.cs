using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using JW.Vepix.Core.Models;

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

        private void pictureList_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListView picturesList = sender as ListView;
            if (picturesList != null && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var selectedItems = picturesList.SelectedItems;
                var pictures = new List<Picture>();
                for( var i = 0; i < selectedItems.Count; i++)
                {
                    pictures.Add(selectedItems[i] as Picture);
                }
                var data = new DataObject(DataFormats.Serializable, pictures); 
                DragDrop.DoDragDrop(picturesList,
                                    data,
                                    DragDropEffects.Move);
            }
        }
    }
}
