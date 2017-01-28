using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public interface IDialogService
    {
        void ShowVepixDialog(Window window);
        void CloseVepixDialog();
    }
}
