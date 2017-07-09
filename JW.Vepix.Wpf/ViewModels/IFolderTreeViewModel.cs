using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IFolderTreeViewModel
    {
        ObservableCollection<IFolderTreeItemViewModel> FolderItemViewModels { get; }
        bool TryLoad(string folder);
    }
}
