using Jw.Data;
using Jw.Vepix.Wpf.Messages;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class ThumbnailGridViewModel : ViewModelBase
    {
        public ObservableCollection<Picture> Pictures
        {
            get
            {
                if (_filterOn)
                {
                    return new ObservableCollection<Picture>(
                        _pictures.Where(pic => pic.ImageName.Contains(_searchFilter)).ToList());
                }
                else
                {
                    return _pictures;
                }
            }
            set
            {
                _pictures = value;
                NotifyPropertyChanged();
            }
        }

        public Picture SelectedPicture { get; private set; }

        public ThumbnailGridViewModel(IDialogService modalDialog)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictures = new ObservableCollection<Picture>();
            _filterOn = false;
            _searchFilter = string.Empty;
            _pictureRepo = new PictureRepository();
            _modalDialog = modalDialog;

            EditImageNameCommand = new RelayCommand<Picture>(OnEditImageName);
            CropImageCommand = new RelayCommand<Picture>(OnCropImage);
            DeleteImageCommand = new RelayCommand<Picture>(OnDeleteImage);
            CloseImageCommand = new RelayCommand<Picture>(OnCloseImage);

            Messenger.Default.Register<ObservableCollection<Picture>>(this, OnPicturesLoaded);
            // todo: find out how to properly register multiple messages. Does having more than one Register
            //       need a context (3rd parameter?)
            // without it, the message will not be added to the Messenger Dictionary.
            Messenger.Default.Register<UpdatePictureNameMessage>(this, OnPictureNameChanged, "NameChange");
            Messenger.Default.Register<SearchFilterMessage>(this, OnSearchFilter, "SearchFilter");
        }

        public RelayCommand<Picture> EditImageNameCommand { get; private set; }
        public RelayCommand<Picture> CropImageCommand { get; private set; }
        public RelayCommand<Picture> DeleteImageCommand { get; private set; }
        public RelayCommand<Picture> CloseImageCommand { get; private set; }

        private void OnEditImageName(Picture picture)
        {
            SelectedPicture = picture;
            Messenger.Default.Send<Picture>(picture);
            _modalDialog.ShowVepixDialog(new Views.EditNameDialogView());
        }

        private void OnCropImage(Picture picture)
        {
            System.Windows.MessageBox.Show("Crop: " + picture.FullFileName);
        }

        private void OnDeleteImage(Picture picture)
        {
            if (System.Windows.MessageBox.Show(
                string.Format("Are you sure you want to delete this image:\n\n\"{0}\"\n", picture.FullFileName), 
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

        private void OnPicturesLoaded(ObservableCollection<Picture> pictures)
        {
            Pictures = pictures;
        }

        private void OnPictureNameChanged(UpdatePictureNameMessage newPictureName)
        {
            // is there a better way of doing this? maybe just pass in the object instead.
            var index = _pictures.IndexOf(SelectedPicture);
            _pictures.RemoveAt(index);
            SelectedPicture.FullFileName = 
                SelectedPicture.FolderPath + newPictureName.NewName + SelectedPicture.FileExtension;
            _pictures.Insert(index, SelectedPicture);
            NotifyPropertyChanged("Pictures");
        }

        private void OnSearchFilter(SearchFilterMessage searchFilterMessage)
        {
            if (searchFilterMessage.Text == string.Empty)
            {
                _filterOn = false;
                Pictures = _pictures;
            }
            else
            {
                _filterOn = true;
                _searchFilter = searchFilterMessage.Text;
                NotifyPropertyChanged("Pictures");
            }
        }

        private ObservableCollection<Picture> _pictures;
        private bool _filterOn;
        private string _searchFilter;
        private IPictureRepository _pictureRepo;
        private IDialogService _modalDialog;
    }
}
