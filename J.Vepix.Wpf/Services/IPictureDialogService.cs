using J.Vepix.Core.Models;
using Prism.Events;

namespace J.Vepix.Wpf.Services
{
    interface IPictureDialogService
    {
        void ShowVepixDialog(IEventAggregator eventAggregator, Picture pictures);
        void CloseVepixDialog();
    }
}
