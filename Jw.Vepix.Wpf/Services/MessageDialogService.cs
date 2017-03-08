using System.Windows;
using System.Windows.Forms;

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

        private Window _modalDialog;
    }
}
