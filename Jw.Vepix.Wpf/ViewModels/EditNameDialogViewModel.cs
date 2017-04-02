using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.ComponentModel;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class EditNameDialogViewModel : ViewModelBase
    {
        public string EditPictureName
        {
            get
            {
                return _editPictureName;
            }
            set
            {
                _editPictureName = value;
                NotifyPropertyChanged("EditPictureName");
            }
        }

        public EditNameDialogViewModel(IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            _pictureRepo = new PictureRepository();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<EditPictureNameEvent>().Subscribe(OnNewEditPictureName);

            OkCommand = new RelayCommand<Window>(OnOk);
            CancelCommand = new RelayCommand<Window>(OnCancel);
        }

        private void OnNewEditPictureName(Picture picture)
        {
            _editPicture = picture;
            EditPictureName = picture.ImageName;
        }

        public void ShowDialog(string pictureName)
        {
            EditPictureName = pictureName;
            new Views.EditNameDialogView().ShowDialog();
        }

        public RelayCommand<Window> OkCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }

        public bool? DialogResult
        {
            get
            {
                return true;
            }
        }

        private void OnOk(Window window)
        {
            // todo: check if context(3rd parameter) is needed here.
            if (_pictureRepo.TryChangePictureName(_editPicture, EditPictureName))
            {
                _eventAggregator.GetEvent<UpdatePictureNameEvent>().Publish(EditPictureName);
                window.Close();
            }
            else
            {
                MessageBox.Show("Invalid file name or file name already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancel(Window window)
        {
            window.Close();
        }

        private IPictureRepository _pictureRepo;
        private string _editPictureName;
        private Picture _editPicture;
        private IEventAggregator _eventAggregator;
    }
}
