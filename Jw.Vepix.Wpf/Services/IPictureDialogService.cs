using Jw.Vepix.Data;
using Prism.Events;

namespace Jw.Vepix.Wpf.Services
{
    interface IPictureDialogService
    {
        void ShowVepixDialog(IEventAggregator eventAggregator, Picture pictures);
        void CloseVepixDialog();
    }
}
