using Jw.Data;
using Jw.Vepix.Wpf.Messages;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
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

        public EditNameDialogViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            _pictureRepo = new PictureRepository();

            Messenger.Default.Register<Picture>(this, pic =>
            {
                _editPicture = pic;
                EditPictureName = pic.ImageName;
            });

            OkCommand = new RelayCommand<Window>(OnOk);
            CancelCommand = new RelayCommand<Window>(OnCancel);
        }

        public void ShowDialog(string pictureName)
        {
            EditPictureName = pictureName;
            new Views.EditNameDialogView().ShowDialog();
        }

        public RelayCommand<Window> OkCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }

        private void OnOk(Window window)
        {
            // todo: check if context(3rd parameter) is needed here.
            if (_pictureRepo.TryChangePictureName(_editPicture, EditPictureName))
            {
                Messenger.Default.Send<UpdatePictureNameMessage>(new UpdatePictureNameMessage(EditPictureName), "NameChange");
                window.Close();
            }
            else
            {
                // invalid file name or file name already exists
            }
        }

        private void OnCancel(Window window)
        {
            window.Close();
        }

        private IPictureRepository _pictureRepo;
        private string _editPictureName;
        private Picture _editPicture;
    }
}
