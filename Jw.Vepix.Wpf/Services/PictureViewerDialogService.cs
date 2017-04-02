using Jw.Vepix.Data;
using Jw.Vepix.Wpf.ViewModels;
using Jw.Vepix.Wpf.Views;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Services
{
    public class PictureViewerDialogService : ICollectionDialogService
    {
        public void CloseVepixDialog()
        {
            throw new NotImplementedException();
        }

        public void ShowVepixDialog(IEventAggregator eventAggregator, List<Picture> pictures, int picIndex = -1) //-1 means do not select a picture. In this case just go to the first picture
        {
            var vm = new PictureDialogViewModel(eventAggregator, pictures, picIndex);
            var dialog = new PicturesDialogView();
            dialog.DataContext = vm;
            dialog.Show();
        }
    }
}
