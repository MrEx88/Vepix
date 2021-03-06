﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace JW.Vepix.Wpf.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        private Window _modalDialog;
        private MetroWindow _metroWindow;

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

        public async Task<string> ShowInput(string title, string message, string defaultInput = "")
        {
            var result = await _metroWindow.ShowInputAsync(
                title, message, new MetroDialogSettings() { DefaultText = defaultInput });
            
            return result;
        }
    }
}
