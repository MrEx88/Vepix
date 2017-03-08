using Jw.Data;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Jw.Vepix.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PictureView.xaml
    ///// </summary>
    public partial class PictureView : UserControl
    {
        public PictureView()
        {
            InitializeComponent();
        }

        public PictureView(List<Picture> pictures, Guid guid)
        {

        }
    }
}
