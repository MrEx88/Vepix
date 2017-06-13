using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public interface IMessageDialogService
    {
        void ShowVepixDialog(Window window);
        void CloseVepixDialog();
        void ShowMessage(string title, string message);
        bool ShowQuestion(string message, string title);
    }
}
