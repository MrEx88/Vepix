using Jw.Vepix.Core;
using Jw.Vepix.Core.Interfaces;
using Jw.Vepix.Core.Models;
using Jw.Vepix.Core.Services;
using Jw.Vepix.Wpf.Controls;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PicturesViewerViewModel : ViewModelBase, ICollectionViewModel
    {
        public PicturesViewerViewModel(IPictureRepository pictureRepository, IMessageDialogService messageDialogService,
            IEventAggregator eventAggregator)
        {
            _pictureRepository = pictureRepository;
            _messageDialogService = messageDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CropAreaDrawnEvent>().Subscribe(OnCropAreaDrawn);

            SaveCommand = new RelayCommand<object>(OnSaveExecute, OnSaveCanExecute);
            SaveAsCommand = new RelayCommand<object>(OnSaveAsExecute, OnSaveCanExecute);

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
                }
            }
        }

        public CropSelectionCanvas CropCanvas
        {
            get { return _cropCanvas; }
            set
            {
                if (value != _cropCanvas)
                {
                    _cropCanvas = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Int32Rect? CropArea
        {
            get { return _cropArea; }
            set
            {
                _cropArea = value;
                SaveAsCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double _cropHeight;

        public double CropHeight
        {
            get { return _cropHeight; }
            set
            {
                if (value != _cropHeight)
                {
                    _cropHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _cropWidth;

        public double CropWidth
        {
            get { return _cropWidth; }
            set
            {
                if (value != _cropWidth)
                {
                    _cropWidth = value;
                    NotifyPropertyChanged();
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

        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> SaveAsCommand { get; set; }

        public string ViewTitle
        {
            get { return _viewTitle; }
            private set
            {
                if (value != _viewTitle)
                {
                    _viewTitle = value;
                    NotifyPropertyChanged();
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
            //todo create picturerepository method for this
            FileService fileService = new FileService();
            fileService.SaveImageAs(croppedPicture.BitmapImage, BitmapEncoderType.JPEG);
        }

        private bool OnSaveCanExecute() => _cropArea.HasValue;

        private void SetupCanvas(Picture picture)
        {
            CropHeight = Convert.ToDouble(picture.Height);
            CropWidth = Convert.ToDouble(picture.Width);
        }

        private void OnCropAreaDrawn(Int32Rect rect)
        {
            _cropArea = rect;
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void Load(List<Picture> pictures)
        {
            Pictures = new ObservableCollection<Picture>(pictures);
            ViewingPicture = pictures[0];
            ViewTitle = ViewingPicture.ImageName;

            SetupCanvas(ViewingPicture);
        }

        private IPictureRepository _pictureRepository;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<Picture> _pictures;
        private Int32Rect? _cropArea;
        private Picture _viewingPicture;
        private string _viewTitle;
        private double _zoomFactor;
        private CropSelectionCanvas _cropCanvas;
    }
}
