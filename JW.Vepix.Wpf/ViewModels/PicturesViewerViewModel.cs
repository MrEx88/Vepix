using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Core.Services;
using JW.Vepix.Wpf.Controls;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace JW.Vepix.Wpf.ViewModels
{
    public class PicturesViewerViewModel : ViewModelBase, ICollectionViewModel
    {
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
                }
            }
        }

        // todo: See if I need any of these when I fix the logic for the
        //       Zooming with the slider.
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
        // END

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

            SetupCanvas(ViewingPicture);
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
