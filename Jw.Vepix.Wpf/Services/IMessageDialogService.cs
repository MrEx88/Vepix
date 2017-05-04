using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public interface IMessageDialogService
    {
        void ShowVepixDialog(Window window);
        void CloseVepixDialog();
        bool ShowQuestion(string message, string title);
    }
}
