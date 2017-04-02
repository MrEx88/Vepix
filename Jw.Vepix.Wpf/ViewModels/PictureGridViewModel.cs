using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase
    {
        public ObservableCollection<Picture> Pictures
        {
            get
            {
                return _filterOn ?
                    new ObservableCollection<Picture>(
                        _pictures.Where(pic => pic.ImageName.Contains(_searchFilter)).ToList())
                        : _pictures;
                //if (_filterOn)
                //{
                //    return new ObservableCollection<Picture>(
                //        _pictures.Where(pic => pic.ImageName.Contains(_searchFilter)).ToList());
                //}
                //else
                //{
                //    return _pictures;
                //}
            }
            set
            {
                _pictures = value;
                NotifyPropertyChanged();
            }
        }

        public Picture SelectedPicture { get; private set; }

        public IEventAggregator _eventAggregator;
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

            EditImageNameCommand = new RelayCommand<Picture>(OnEditImageName);
            CropImageCommand = new RelayCommand<Picture>(OnCropImage);
            DeleteImageCommand = new RelayCommand<Picture>(OnDeleteImage);
            CloseImageCommand = new RelayCommand<Picture>(OnCloseImage);

            _eventAggregator.GetEvent<NewPicturesCollectionEvent>().Subscribe(OnPicturesLoaded);
            _eventAggregator.GetEvent<SearchFilterEvent>().Subscribe(OnSearchFilter);
            _eventAggregator.GetEvent<UpdatePictureNameEvent>().Subscribe(OnPictureNameChanged);
        }

        public RelayCommand<Picture> EditImageNameCommand { get; private set; }
        public RelayCommand<Picture> CropImageCommand { get; private set; }
        public RelayCommand<Picture> DeleteImageCommand { get; private set; }
        public RelayCommand<Picture> CloseImageCommand { get; private set; }

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

        private void OnPicturesLoaded(List<Picture> pictures)
        {
            //pictures.ForEach(pic => Pictures.Add(pic));
            Pictures = new ObservableCollection<Picture>(pictures);
        }

        private void OnSearchFilter(string searchFilterMessage)
        {
            if (searchFilterMessage == string.Empty)
            {
                _filterOn = false;
                Pictures = _pictures;
            }
            else
            {
                _filterOn = true;
                _searchFilter = searchFilterMessage;
                NotifyPropertyChanged("Pictures");
            }
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

        private ObservableCollection<Picture> _pictures;
        private bool _filterOn;
        private string _searchFilter;
        private IPictureRepository _pictureRepo;
        private IMessageDialogService _modalDialog;
    }
}
