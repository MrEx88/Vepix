using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface IPictureFolderTreeItemViewModel
    {
        IPictureFolderTreeItemViewModel Parent { get; set; }
        ObservableCollection<IPictureFolderTreeItemViewModel> Children { get; set; }
        string FolderName { get; }
        bool IsExpanded { get; set; }
    }
}
