using Jw.Data;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Services
{
    interface ICollectionDialogService
    {
        void ShowVepixDialog(List<Picture> pictures, int picIndex);
        void CloseVepixDialog();
    }
}
