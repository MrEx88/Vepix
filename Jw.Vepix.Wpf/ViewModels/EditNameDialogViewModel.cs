using Jw.Vepix.Data;
using Jw.Vepix.Data.Payloads;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class EditNameDialogViewModel : ViewModelBase
    {
        public EditNameDialogViewModel(IPictureRepository pictureRepository,
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService, List<Picture> pictures)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            _pictureRepo = pictureRepository;

            _eventAggregator = eventAggregator;

            _messageDialogService = messageDialogService;
            _pictures = pictures;
            
            EditPictureName = pictures[0].ImageName;
            _editPicture = pictures[0];

            OkCommand = new RelayCommand<Window>(OnOk);
            CancelCommand = new RelayCommand<Window>(OnCancel);
        }

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

        public List<Picture> Pitcures
        {
            get
            {
                // todo: implement this instead of _editPicture
                return _pictures;
            }
        }

        public bool? DialogResult
        {
            get
            {
                return true;
            }
        }

        public RelayCommand<Window> OkCommand { get; private set; }
        public RelayCommand<Window> CancelCommand { get; private set; }
        
        private void OnOk(Window window)
        {
            if (_pictureRepo.TryChangePictureName(_editPicture, EditPictureName))
            {
                _eventAggregator.GetEvent<PictureNameChangedEvent>().Publish(new PictureNameChangePayload()
                {
                    Guid = _editPicture.Guid,
                    NewPictureName = EditPictureName
                });
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
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private Picture _editPicture;
        private List<Picture> _pictures;
        private string _editPictureName;     
    }
}
