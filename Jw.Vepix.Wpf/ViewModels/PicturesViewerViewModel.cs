using Jw.Vepix.Core;
using Jw.Vepix.Core.Extensions;
using Jw.Vepix.Core.Interfaces;
using Jw.Vepix.Core.Models;
using Jw.Vepix.Core.Services;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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

            _pictures = new ObservableCollection<Picture>();
        }

        public Image Image { get; private set; }
        public Picture ViewingPicture { get; set; }
        public CropSelectionCanvas CropCanvas { get; set; }

        public ObservableCollection<Picture> Pictures
        {
            get
            {
                return _pictures;
            }

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

        private void OnSaveExecute()
        {
            var croppedPicture = BitmapService.CropPreview(ViewingPicture, _cropArea.Value);
            if (_messageDialogService.ShowQuestion("Are you sure you want to overwrite this picture?", "Overwrite Picture?"))
            {
                _pictureRepository.TryOverWrite(croppedPicture, ViewingPicture.FullFileName, ViewingPicture.FileExtension.ToEncoderType());
                _eventAggregator.GetEvent<PictureOverwrittenEvent>().Publish(ViewingPicture.Guid);
            }
        }

        private void OnSaveAsExecute()
        {
            var croppedPicture = BitmapService.CropPreview(ViewingPicture, _cropArea.Value);
            //todo create picturerepository method for this
            FileService fileService = new FileService();
            fileService.SaveImageAs(croppedPicture, BitmapEncoderType.JPEG);
        }

        private bool OnSaveCanExecute() => _cropArea.HasValue;

        private void SetupCanvas(Picture picture)
        {
            Image = new Image()
            {
                Source = picture.BitmapImage,
                Height = picture.Height,
                Width = picture.Width
            };

            CropCanvas = new CropSelectionCanvas(_eventAggregator,
                new PointBoundaries(new Point(picture.Width, picture.Height)));
            CropCanvas.Children.Add(Image);
            CropCanvas.Height = Convert.ToDouble(picture.Height);
            CropCanvas.Width = Convert.ToDouble(picture.Width);
        }

        private void OnCropAreaDrawn(Int32Rect rect)
        {
            _cropArea = rect;
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void Load(List<Picture> pictures)
        {
            _pictures = new ObservableCollection<Picture>(pictures);
            ViewingPicture = pictures[0];
            SetupCanvas(ViewingPicture);
        }

        private IPictureRepository _pictureRepository;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<Picture> _pictures;
        private Int32Rect? _cropArea;
    }
}
