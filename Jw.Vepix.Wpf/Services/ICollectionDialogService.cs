using Jw.Vepix.Data;
using Prism.Events;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Services
{
    interface ICollectionDialogService
    {
        void ShowVepixDialog(IEventAggregator eventAggregator, List<Picture> pictures, int picIndex);
        void CloseVepixDialog();
    }
}
