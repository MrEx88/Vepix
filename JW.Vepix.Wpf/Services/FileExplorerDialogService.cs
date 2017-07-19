using System.Windows.Forms;

namespace JW.Vepix.Wpf.Services
{
    public class FileExplorerDialogService : IFileExplorerDialogService
    {
        public bool ShowFolderBrowserDialog(out string folderPath)
        {
            var folderDialog = new FolderBrowserDialog();
            var result = false;
            result = folderDialog.ShowDialog() == DialogResult.OK;
            folderPath = result ? folderDialog.SelectedPath : string.Empty;
            return result;
        }

        public bool ShowOpenFileDialog(out string[] fileNames)
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.wmp"
            };
            var result = false;
            result = fileDialog.ShowDialog() == DialogResult.OK;
            fileNames = result ? fileDialog.FileNames : new string[] { };
            return result;
        }
    }
}
