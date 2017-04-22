using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    //todo: test out and implement
    public interface IPictureFolderTreeItemViewModel
    {
        IPictureFolderTreeItemViewModel Parent { get; }
        ObservableCollection<IPictureFolderTreeItemViewModel> Children { get; }
        string FolderName { get; }
    }
}
