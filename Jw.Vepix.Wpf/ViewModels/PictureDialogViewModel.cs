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
        public PictureDialogViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            List<Picture> pictures)
        {
            // in future, maybe use this viewer to navigate through photos
            //_pictures = pictures;
            //_selectedPicture = selectedPicture;
            _pictureRepo = new PictureRepository(); //todo

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CropAreaDrawnEvent>().Subscribe(OnCropAreaDrawn);

            _messageDialogService = messageDialogService;

            ViewingPicture = pictures[0];
            SetupCanvas(eventAggregator, ViewingPicture);

            SaveCommand = new RelayCommand<object>(OnSaveExecute, OnSaveCanExecute);
            SaveAsCommand = new RelayCommand<object>(OnSaveAsExecute, OnSaveCanExecute);
        }

        public Picture ViewingPicture { get; set; }
        public System.Windows.Controls.Image FullImage { get; set; } //maybe just make local to method
        public List<Picture> Pitcures
        {
            // in future, maybe use this viewer to navigate through photos
            get
            {
                throw new NotImplementedException();
            }
        }
        public CropSelectionCanvas CropCanvas { get; set; }

        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> SaveAsCommand { get; set; }

        private void OnSaveExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value);
            if (_messageDialogService.ShowQuestion("Are you sure you want to overwrite this image?", "Overwrite Image?"))
            {
                _pictureRepo.TryOverWrite(croppedImage, ViewingPicture.FullFileName, ViewingPicture.FileExtension.ToEncoderType());
                _eventAggregator.GetEvent<PictureOverwrittenEvent>().Publish(ViewingPicture.Guid);
            }
        }

        private void OnSaveAsExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value);
            //todo create picturerepository method for this
            FileService fileService = new FileService();
            fileService.SaveImageAs(croppedImage, BitmapEncoderType.JPEG);            
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
        private IMessageDialogService _messageDialogService;
        private PictureRepository _pictureRepo;
        //private List<Picture> _pictures;
        //private int _picIndex;
    }
}
