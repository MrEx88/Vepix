using J.Vepix.Wpf.Events;
using J.Vepix.Wpf.Payloads;
using J.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace J.Vepix.Wpf.ViewModels
{
    public class PictureFolderTreeItemViewModel : ViewModelBase, IPictureFolderTreeItemViewModel
    {
        public PictureFolderTreeItemViewModel(DirectoryInfo dirInfo, IEventAggregator eventAggregator)
            : this(dirInfo, parent: null, eventAggregator: eventAggregator)
        {
        }

        private PictureFolderTreeItemViewModel(DirectoryInfo dirInfo, PictureFolderTreeItemViewModel parent,
            IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _absolutePath = dirInfo.FullName;
            _parent = parent;

            _eventAggregator = eventAggregator;

            _children = new ObservableCollection<IPictureFolderTreeItemViewModel>(
                (from child in dirInfo.GetDirectories()
                 select new PictureFolderTreeItemViewModel(child, parent: this, eventAggregator: eventAggregator)).ToList());
            
            _filePattern = "*.*";
            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("FolderName");

            OpenPicturesInFolderCommand = new RelayCommand<object>(OnOpenPicturesInFolder);
        }

        public IPictureFolderTreeItemViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ObservableCollection<IPictureFolderTreeItemViewModel> Children
        {
            get { return _children; }
            set
            {
                if (value != _children)
                {
                    _children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool TreeItemAlreadyExists(string absolutePath)
        {
            var directories = new DirectoryInfo(AbsolutePath).GetDirectories().ToList();
            return absolutePath == AbsolutePath || directories.Exists(dic => dic.FullName == absolutePath);
        }

        public bool IsAParentTo(PictureFolderTreeItemViewModel treeItem)
        {
            var parent = new DirectoryInfo(AbsolutePath).Parent;
            return parent.FullName == treeItem.AbsolutePath;
        }

        public string FolderName => new DirectoryInfo(AbsolutePath).Name;

        public string AbsolutePath
        {
            get { return _absolutePath; }
            set
            {
                if (value != _absolutePath)
                {
                    _absolutePath = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("FolderName");
                }
            }
        }

        //not sure if i want to keep this; i think I will  just have a textbox in the mainview (or foldertreeview) for this
        public string FilePattern
        {
            get { return _filePattern; }
            set { _filePattern = value; }
        }

        public bool IsParent => _parent == null;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged();
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                {
                    _parent.IsExpanded = true;
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RelayCommand<object> OpenPicturesInFolderCommand { get; private set; }

        private void OnOpenPicturesInFolder(object folder)
        {
            var folderName = ((PictureFolderTreeItemViewModel)folder).AbsolutePath;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Publish(new PicturesFolderPayload()
            {
                AbsolutePath = _absolutePath,
                FilePattern = _filePattern
            });
        }

        private IPictureFolderTreeItemViewModel _parent;
        private ObservableCollection<IPictureFolderTreeItemViewModel> _children;
        private IEventAggregator _eventAggregator;
        private string _absolutePath;
        private bool _isExpanded;
        private bool _isSelected;
        private string _filePattern; // ex: "*12.jpg, *.bmp"
    }
}
