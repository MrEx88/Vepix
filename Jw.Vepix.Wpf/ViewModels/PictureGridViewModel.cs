using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase, IPictureGridViewModel
    {
        public PictureGridViewModel(IMessageDialogService modalDialog, IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictures = new ObservableCollection<Picture>();
            _filterOn = false;
            _searchFilter = string.Empty;
            _pictureRepo = new PictureRepository();
            _modalDialog = modalDialog;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UpdatePictureNameEvent>().Subscribe(OnPictureNameChanged);

            SearchCommand = new RelayCommand<string>(OnSearch);
            EditImageNameCommand = new RelayCommand<Picture>(OnEditImageName);
            CropImageCommand = new RelayCommand<Picture>(OnCropImage);
            DeleteImageCommand = new RelayCommand<Picture>(OnDeleteImage);
            CloseImageCommand = new RelayCommand<Picture>(OnCloseImage);            
        }

        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Picture> Pictures
        {
            get
            {
                return _filterOn ?
                    new ObservableCollection<Picture>(
                        _pictures.Where(pic => pic.ImageName.Contains(_searchFilter)).ToList())
                        : _pictures;
            }
            set
            {
                _pictures = value;
                NotifyPropertyChanged();
            }
        }

        public Picture SelectedPicture { get; private set; }
        
        public RelayCommand<string> SearchCommand { get; private set; }
        public RelayCommand<Picture> EditImageNameCommand { get; private set; }
        public RelayCommand<Picture> CropImageCommand { get; private set; }
        public RelayCommand<Picture> DeleteImageCommand { get; private set; }
        public RelayCommand<Picture> CloseImageCommand { get; private set; }

        private void OnSearch(string searchFilter)
        {
            if (searchFilter == string.Empty)
            {
                _filterOn = false;
                Pictures = _pictures;
            }
            else
            {
                _filterOn = true;
                _searchFilter = searchFilter;
                NotifyPropertyChanged("Pictures");
            }
        }

        private void OnEditImageName(Picture picture)
        {
            SelectedPicture = picture;
            // todo: find another way to handle dialogs
            _eventAggregator.GetEvent<EditPictureNameEvent>().Publish(picture);
            _modalDialog.ShowVepixDialog(new Views.EditNameDialogView());
        }

        private void OnCropImage(Picture picture)
        {
            // maybe do this instead of what I have above. Also need to do IoC here
            // todo: 
            //          Implement in OnEditImageName()
            //          Implement in VepixWindowViewModel::OnAbout()
            //          Implement in VepixWindowViewModel::OnDeleteImage()
            ICollectionDialogService dialog = new PictureViewerDialogService();
            dialog.ShowVepixDialog(_eventAggregator, Pictures.ToList(), Pictures.IndexOf(picture));
        }

        private void OnDeleteImage(Picture picture)
        {
            if (System.Windows.MessageBox.Show(
                $"Are you sure you want to delete this image:\n\n\"{picture.FullFileName}\"\n",
                "Delete Image?",
                System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                Pictures.Remove(picture);
                _pictureRepo.TryDelete(picture);
            }
        }

        private void OnCloseImage(Picture picture)
        {
            Pictures.Remove(picture);
        }

        private void OnPictureNameChanged(string newName)
        {
            var index = _pictures.IndexOf(SelectedPicture);
            _pictures.RemoveAt(index);
            SelectedPicture.FullFileName =
                SelectedPicture.FolderPath + newName + SelectedPicture.FileExtension;
            _pictures.Insert(index, SelectedPicture);
            NotifyPropertyChanged("Pictures");
        }

        public void Load(List<Picture> pictures)
        {
            if (pictures.Count > 0)
                FolderName = new DirectoryInfo(pictures[0].FolderPath).Name;
            Pictures = new ObservableCollection<Picture>(pictures);
        }

        private IEventAggregator _eventAggregator;
        private IPictureRepository _pictureRepo;
        private IMessageDialogService _modalDialog;
        private ObservableCollection<Picture> _pictures;
        private string _folderName;
        private bool _filterOn;
        private string _searchFilter;        
    }
}
