using System.IO;
using System.Windows.Forms;

namespace J.Vepix.Wpf.Services
{
    public interface IFileExplorerDialogService
    {
        DialogResult ShowFolderBrowserDialog(out string folderPath);
        DialogResult ShowOpenFileDialog(out string[] fileNames);
    }
}
