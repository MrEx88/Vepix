using JW.Vepix.Core.Models;
using Prism.Events;

namespace JW.Vepix.Wpf.Services
{
    interface IPictureDialogService
    {
        void ShowVepixDialog(IEventAggregator eventAggregator, Picture pictures);
        void CloseVepixDialog();
    }
}
