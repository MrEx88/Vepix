using JW.Vepix.Core.Extensions;
using Prism.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace JW.Vepix.Wpf.ViewModels
{
    public class FolderTreeViewModel : ViewModelBase, IFolderTreeViewModel
    {
        public FolderTreeViewModel(IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _eventAggregator = eventAggregator;

            _folderTreeItemViewModels = new ObservableCollection<IFolderTreeItemViewModel>();
        }

        public ObservableCollection<IFolderTreeItemViewModel> FolderItemViewModels
        {
            get { return _folderTreeItemViewModels; }
            set
            {
                _folderTreeItemViewModels = value;
                NotifyPropertyChanged();
            }
        }

        public bool TryLoad(string folder)
        {
            var dirInfo = new DirectoryInfo(folder);

            var treeItem = new FolderTreeItemViewModel(dirInfo, _eventAggregator);
            return TryAdd(treeItem);
        }

        private bool TryAdd(FolderTreeItemViewModel treeItem)
        {
            if (FolderItemViewModels.Count != 0)
            {
                // Check if folder already exists.
                if (FolderItemViewModels.ToList().Exists(folderTreeItem => 
                        folderTreeItem.TreeItemAlreadyExists(treeItem.AbsolutePath)))
                {
                    return false;
                }

                // Remove if folder is Parent to existing folder(s).
                FolderItemViewModels.RemoveAll(folderItem => folderItem.IsAParentTo(treeItem));
            }

            FolderItemViewModels.Add(treeItem);

            return true;
        }

        private ObservableCollection<IFolderTreeItemViewModel> _folderTreeItemViewModels;
        private IEventAggregator _eventAggregator;
    }
}
