using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogService()
        {
            _metroWindow = (MetroWindow)Application.Current.MainWindow;
        }

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

        public async void ShowMessage(string title, string message) => await _metroWindow.ShowMessageAsync(
            title, message, MessageDialogStyle.Affirmative);

        public async void ShowInput(string title, string message) => await _metroWindow.ShowInputAsync(
            title, message, new MetroDialogSettings());

        private Window _modalDialog;
        private MetroWindow _metroWindow;
    }
}
