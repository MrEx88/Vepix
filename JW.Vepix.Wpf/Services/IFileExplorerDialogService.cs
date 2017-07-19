
namespace JW.Vepix.Wpf.Services
{
    public interface IFileExplorerDialogService
    {
        bool ShowFolderBrowserDialog(out string folderPath);
        bool ShowOpenFileDialog(out string[] fileNames);
    }
}
