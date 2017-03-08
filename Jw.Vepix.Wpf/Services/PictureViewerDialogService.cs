using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jw.Data;
using Jw.Vepix.Wpf.ViewModels;
using Jw.Vepix.Wpf.Views;

namespace Jw.Vepix.Wpf.Services
{
    public class PictureViewerDialogService : ICollectionDialogService
    {
        public void CloseVepixDialog()
        {
            throw new NotImplementedException();
        }

        public void ShowVepixDialog(List<Picture> pictures, int picIndex = -1) //-1 means do not select a picture. In this case just go to the first picture
        {
            var vm = new PictureViewModel(pictures, picIndex);
            var dialog = new PicturesDialogView();
            dialog.DataContext = vm;
            dialog.Show();
        }
    }
}
