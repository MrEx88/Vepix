using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IFolderTreeItemViewModel
    {
        IFolderTreeItemViewModel Parent { get; set; }
        ObservableCollection<IFolderTreeItemViewModel> Children { get; set; }
        bool IsExpanded { get; set; }
        bool TreeItemAlreadyExists(string absolutePath);
        bool IsAParentTo(FolderTreeItemViewModel treeItem);
    }
}
