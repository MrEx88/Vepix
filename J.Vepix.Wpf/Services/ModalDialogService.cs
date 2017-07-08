using Jw.Vepix.Wpf.Messages;
using Jw.Vepix.Wpf.Utilities;
using System.Windows;
using System.Windows.Forms;

namespace Jw.Vepix.Wpf.Services
{
    public class ModalDialogService : IDialogService
    {
        public void ShowVepixDialog(Window window)
        {
            _modalDialog = window;
            _modalDialog.ShowDialog();
        }

        public void ShowDialog(FolderBrowserDialog dialog)
        {
            dialog.ShowDialog();
            //return dialog.SelectedPath;
        }

        public void ShowDialog(OpenFileDialog dialog)
        {
            dialog.Multiselect = true;
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
            dialog.ShowDialog();
            //return dialog.FileNames.ToList();
        }

        public void CloseVepixDialog()
        {
            Messenger.Default.Send<UpdatePictureNameMessage>(new UpdatePictureNameMessage("something"));
            _modalDialog?.Close();
        }

        private Window _modalDialog;
    }
}
