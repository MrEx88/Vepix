using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Payloads;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System;
using System.Windows;
using JW.Vepix.Core.Models;
using System.Collections.Generic;

namespace JW.Vepix.Wpf.ViewModels
{
    public class FolderTreeItemViewModel : ViewModelBase, IFolderTreeItemViewModel
    {
        private IFolderTreeItemViewModel _parent;
        private ObservableCollection<IFolderTreeItemViewModel> _children;
        private IEventAggregator _eventAggregator;
        private string _absolutePath;
        private bool _isExpanded;
        private bool _isSelected;

        public FolderTreeItemViewModel(DirectoryInfo dirInfo, IEventAggregator eventAggregator)
            : this(dirInfo, parent: null, eventAggregator: eventAggregator)
        {
        }

        private FolderTreeItemViewModel(DirectoryInfo dirInfo, FolderTreeItemViewModel parent,
            IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            _absolutePath = dirInfo.FullName;
            _parent = parent;

            _eventAggregator = eventAggregator;

            _children = new ObservableCollection<IFolderTreeItemViewModel>(
                (from child in dirInfo.GetDirectories()
                 select new FolderTreeItemViewModel(child, parent: this, eventAggregator: eventAggregator)).ToList());
            
            NotifyPropertyChanged(() => Children);
            NotifyPropertyChanged(() => ViewTitle);

            OpenPicturesInFolderCommand = new RelayCommand<object>(OnOpenPicturesInFolder);
            DropCommand = new RelayCommand<object>(OnDropCommand);
        }

        public IFolderTreeItemViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ObservableCollection<IFolderTreeItemViewModel> Children
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

        public bool IsAParentTo(FolderTreeItemViewModel treeItem)
        {
            var parent = new DirectoryInfo(AbsolutePath).Parent;
            return parent.FullName == treeItem.AbsolutePath;
        }

        public override string ViewTitle => new DirectoryInfo(AbsolutePath).Name;

        public string AbsolutePath
        {
            get { return _absolutePath; }
            set
            {
                if (value != _absolutePath)
                {
                    _absolutePath = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => ViewTitle);
                }
            }
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
        public RelayCommand<object> DropCommand { get; private set; }

        private void OnOpenPicturesInFolder(object folder)
        {
            var absolutePath = ((FolderTreeItemViewModel)folder).AbsolutePath;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Publish(new PicturesFolderPayload()
            {
                AbsolutePath = absolutePath
            });
        }

        private void OnDropCommand(object inObject)
        {
            IDataObject ido = inObject as IDataObject;

            var data = (List<Picture>)ido.GetData(DataFormats.Serializable);
            if (data != null)
            {
                _eventAggregator.GetEvent<MovingPicturesEvent>()
                    .Publish(new MovingPicturesPayload()
                    {
                        Pictures = data,
                        NewFolderPath = AbsolutePath
                    });
            }
        }
    }
}
