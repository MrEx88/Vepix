using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Jw.Vepix.Wpf.Utilities
{
    public class CropSelectionCanvas : Canvas
    {
        public CropSelectionCanvas(IEventAggregator eventAggregator, PointBoundaries pictureBoundaries)
        {
            _eventAggregator = eventAggregator;
            _pointBoundaries = pictureBoundaries;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!IsMouseCaptured)
            {
                _mouseDownPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (IsMouseCaptured && _cropShape != null)
            {
                ReleaseMouseCapture();
                _eventAggregator.GetEvent<CropAreaDrawnEvent>().Publish(_cropDimensions);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsMouseCaptured)
            {
                var currentPoint = e.GetPosition(this);
                if (_cropShape == null)
                {
                    CreateCropArea();
                }

                var width = Math.Abs(_mouseDownPoint.X - currentPoint.X);
                var height = Math.Abs(_mouseDownPoint.Y - currentPoint.Y);
                var left = _pointBoundaries.GetXPointWithinBoundary(Math.Min(_mouseDownPoint.X,
                    currentPoint.X));
                var top = _pointBoundaries.GetXPointWithinBoundary(Math.Min(_mouseDownPoint.Y,
                    currentPoint.Y));

                _cropDimensions = new Int32Rect()
                {
                    X = Convert.ToInt32(left),
                    Y = Convert.ToInt32(top),
                    Width = Convert.ToInt32(width),
                    Height = Convert.ToInt32(height)
                };

                _cropShapeBorder.Width = width + BORDER_PADDING;
                _cropShapeBorder.Height = height + BORDER_PADDING;
                Canvas.SetLeft(_cropShapeBorder, left - BORDER_OFFSET);
                Canvas.SetTop(_cropShapeBorder, top - BORDER_OFFSET);

                _cropShape.Width = width;
                _cropShape.Height = height;
                Canvas.SetLeft(_cropShape, left);
                Canvas.SetTop(_cropShape, top);
            }
        }

        private void CreateCropArea()
        {
            _cropShapeBorder = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.White),
                Opacity = 0.88
            };
            _cropShape = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Opacity = 0.88
            };
            Children.Add(_cropShapeBorder);
            Children.Add(_cropShape);
        }

        private Point _mouseDownPoint;
        private PointBoundaries _pointBoundaries;
        private Shape _cropShapeBorder;
        private Shape _cropShape;
        private Int32Rect _cropDimensions;
        private IEventAggregator _eventAggregator;

        private const double BORDER_PADDING = 2.0;
        private const double BORDER_OFFSET = 1.0;
    }
}
