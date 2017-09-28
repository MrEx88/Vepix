using JW.Vepix.Core.Extensions;
using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace JW.Vepix.Wpf.ViewModels
{
    public class PicturesViewerViewModel : ViewModelBase, IFlyoutViewModel
    {
        private IPictureRepository _pictureRepository;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<Picture> _pictures;
        private Int32Rect? _cropArea;
        private Picture _viewingPicture;
        private string _viewTitle;
        private double _zoomFactor;

        public PicturesViewerViewModel(IPictureRepository pictureRepository,
                                       IMessageDialogService messageDialogService,
                                       IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            _pictureRepository = pictureRepository;
            _messageDialogService = messageDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CropAreaDrawnEvent>().Subscribe(OnCropAreaDrawn);

            SaveCommand = new RelayCommand<object>(OnSaveExecute, OnSaveCanExecute);
            SaveAsCommand = new RelayCommand<object>(OnSaveAsExecute, OnSaveCanExecute);

        }

        public override string ViewTitle
        {
            get { return _viewTitle; }
            protected set
            {
                if (value != _viewTitle)
                {
                    _viewTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Picture ViewingPicture
        {
            get { return _viewingPicture; }
            set
            {
                if (value != _viewingPicture)
                {
                    _viewingPicture = value;
                    NotifyPropertyChanged();

                    ViewTitle = value.ImageName;
                }
            }
        }

        public double ZoomFactor
        {
            get { return _zoomFactor; }
            set
            {
                if (value != _zoomFactor)
                {
                    _zoomFactor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Int32Rect? CropArea
        {
            get { return _cropArea; }
            set
            {
                if (value != _cropArea)
                {
                    _cropArea = value;
                    SaveAsCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged(); 
                }
            }
        }

        public ObservableCollection<Picture> Pictures
        {
            get { return _pictures; }
            set
            {
                if (value != _pictures)
                {
                    _pictures = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void Load(List<Picture> pictures)
        {
            Pictures = new ObservableCollection<Picture>(pictures);
            ViewingPicture = pictures[0];
            ViewTitle = ViewingPicture.ImageName;
        }

        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> SaveAsCommand { get; set; }

        private void OnSaveExecute()
        {
            var croppedPicture = _pictureRepository.GetCroppedImage(ViewingPicture, _cropArea.Value);
            if (_messageDialogService.ShowQuestion("Are you sure you want to overwrite this picture?", "Overwrite Picture?"))
            {
                _pictureRepository.TryOverwrite(croppedPicture);
                _eventAggregator.GetEvent<PictureOverwrittenEvent>().Publish(ViewingPicture.Guid);
            }
        }

        private void OnSaveAsExecute()
        {
            var croppedPicture = _pictureRepository.GetCroppedImage(ViewingPicture, _cropArea.Value);
            _pictureRepository.TrySaveAs(croppedPicture.BitmapImage, croppedPicture.FileExtension.ToEncoderType());
        }

        private bool OnSaveCanExecute() => _cropArea.HasValue;

        private void OnCropAreaDrawn(Int32Rect rect)
        {
            _cropArea = rect;
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
