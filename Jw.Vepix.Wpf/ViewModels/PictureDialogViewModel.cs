using Jw.Vepix.Common;
using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureDialogViewModel : ViewModelBase, IPicturesDialogViewModel
    {
        public Picture ViewingPicture { get; set; }
        public System.Windows.Controls.Image FullImage { get; set; }
        public CropSelectionCanvas CropCanvas { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> SaveAsCommand { get; set; }

        public List<Picture> Pitcures
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public PictureDialogViewModel(IEventAggregator eventAggregator, List<Picture> pictures)
        {
            // in future, maybe use this viewer to navigate through photos
            //_pictures = pictures;
            //_selectedPicture = selectedPicture;

            ViewingPicture = pictures[0];
            SetupCanvas(eventAggregator, ViewingPicture);

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CropAreaDrawnEvent>().Subscribe(OnCropAreaDrawn);

            SaveCommand = new RelayCommand<object>(OnSaveExecute, OnSaveCanExecute);
            SaveAsCommand = new RelayCommand<object>(OnSaveAsExecute, OnSaveCanExecute);
        }

        private void OnSaveAsExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value, BitmapEncoderType.JPEG);
            FileService fileService = new FileService();
            fileService.SaveImageAs(croppedImage, BitmapEncoderType.JPEG);
        }

        private void OnSaveExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value, BitmapEncoderType.JPEG);
            //1 are you sure you want to overwrite?
            //2 FileService fileService = new FileService();
            //fileService.SaveImageOverwrite(croppedImage, BitmapEncoderType.JPEG);
            //3 notifypicturechange
        }

        private bool OnSaveCanExecute() => _cropArea.HasValue;

        private void SetupCanvas(IEventAggregator eventAggregator, Picture picture)
        {
            FullImage = new System.Windows.Controls.Image()
            {
                Source = picture.BitmapImage,
                Height = picture.Height,
                Width = picture.Width
            };
            CropCanvas = new CropSelectionCanvas(eventAggregator, 
                new PointBoundaries(new Point(picture.Width, picture.Height)));
            CropCanvas.Children.Add(FullImage);
            CropCanvas.Height = Convert.ToDouble(picture.Height);
            CropCanvas.Width = Convert.ToDouble(picture.Width);
        }

        private void OnCropAreaDrawn(Int32Rect rect)
        {
            _cropArea = rect;
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private IEventAggregator _eventAggregator;
        private Int32Rect? _cropArea;
        //private List<Picture> _pictures;
        //private int _picIndex;
    }
}
