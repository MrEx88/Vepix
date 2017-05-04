using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public void ShowVepixDialog(Window window)
        {
            _modalDialog = window;
            _modalDialog.ShowDialog();
        }

        public void CloseVepixDialog()
        {
            _modalDialog?.Close();
        }

        public bool ShowQuestion(string message, string title) => (MessageBox.Show(
                message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes);

        public void ShowDialog(string message, string title) => MessageBox.Show(
            message, title);

        private Window _modalDialog;
    }
}
