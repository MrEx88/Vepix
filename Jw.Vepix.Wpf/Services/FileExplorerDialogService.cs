using System.Windows.Forms;

namespace Jw.Vepix.Wpf.Services
{
    public class FileExplorerDialogService : IFileExplorerDialogService
    {
        public DialogResult ShowFolderBrowserDialog(out string folderPath)
        {
            DialogResult result;
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            result = folderDialog.ShowDialog();
            folderPath = result == DialogResult.OK ? folderDialog.SelectedPath : string.Empty;
            return result;
        }

        public DialogResult ShowOpenFileDialog(out string[] fileNames)
        {
            DialogResult result;
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.wmp"
            };
            result = fileDialog.ShowDialog();
            fileNames = result == DialogResult.OK ? fileDialog.FileNames : new string[] { };
            return result;
        }
    }
}
