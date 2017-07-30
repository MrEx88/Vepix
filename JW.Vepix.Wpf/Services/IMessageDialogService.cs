using System;
using System.Threading.Tasks;
using System.Windows;

namespace JW.Vepix.Wpf.Services
{
    public interface IMessageDialogService
    {
        void ShowVepixDialog(Window window);
        void CloseVepixDialog();
        void ShowMessage(string title, string message);
        bool ShowQuestion(string message, string title);
        Task<string> ShowInput(string title, string message, string defaultInput = "");
    }
}
