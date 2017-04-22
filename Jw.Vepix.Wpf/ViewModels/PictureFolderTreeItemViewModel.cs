using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    // maybe I don't need FolderNode class. Just have its properties here. That way
    // I only have to manage one tree, not two.
    public class PictureFolderTreeItemViewModel : ViewModelBase
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

            _children = new ObservableCollection<PictureFolderTreeItemViewModel>(
                (from child in dirInfo.GetDirectories()
                 select new PictureFolderTreeItemViewModel(child, parent: this, eventAggregator: eventAggregator)).ToList());

            _fileFilter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff";
            _filePattern = "*.*";
            NotifyPropertyChanged("Children");
            NotifyPropertyChanged("FolderName");

            OpenPicturesInFolderCommand = new RelayCommand<object>(OnOpenPicturesInFolder);
        }

        // todo: figure out how to change parent of tree
        public PictureFolderTreeItemViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        //todo: figure out how to update a tree
        public ObservableCollection<PictureFolderTreeItemViewModel> Children
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

        //todo: implment by adding filter in constructor parameter. default is "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff"
        public string FileFilter
        {
            get { return _fileFilter; }
            set { _fileFilter = value; }
        }

        //not sure if i want to keep this
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
                FileFilter = _fileFilter,
                FilePattern = _filePattern
            });
        }

        private PictureFolderTreeItemViewModel _parent;
        private ObservableCollection<PictureFolderTreeItemViewModel> _children;
        private IEventAggregator _eventAggregator;
        private string _absolutePath;
        private bool _isExpanded;
        private bool _isSelected;
        private string _fileFilter; // ex: "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff"
        private string _filePattern; // ex: "*12.jpg, *.bmp"
    }
}
