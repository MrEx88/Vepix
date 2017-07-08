using J.Vepix.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace J.Vepix.Wpf.Controls
{
    public class CropSelectionCanvas : Canvas
    {
        public CropSelectionCanvas()
        {
        }

        public Int32Rect? CropArea
        {
            get { return (Int32Rect?)GetValue(CropAreaProperty); }
            set { SetValue(CropAreaProperty, value); }
        }

        public static readonly DependencyProperty CropAreaProperty =
        DependencyProperty.Register(
            "CropArea",
            typeof(Int32Rect?),
            typeof(CropSelectionCanvas),
            new PropertyMetadata(default(Int32Rect?)));

        public bool CropEnabled
        {
            get { return (bool)GetValue(CropEnabledProperty); }
            set { SetValue(CropEnabledProperty, value); }
        }

        private static readonly DependencyProperty CropEnabledProperty =
        DependencyProperty.Register(
            "CropEnabled",
            typeof(bool),
            typeof(CropSelectionCanvas),
            new PropertyMetadata(default(bool)));

        public void ClearCropShape()
        {
            if (Children.Contains(_cropShape))
            {
                Children.Remove(_cropShape);
            }
            _cropShape = null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (CropEnabled && !IsMouseCaptured)
            {
                _mouseDownPoint = e.GetPosition(this);
                CaptureMouse();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (CropEnabled && IsMouseCaptured && _cropShape != null)
            {
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (CropEnabled && IsMouseCaptured)
            {
                var currentPoint = e.GetPosition(this);
                if (_cropShape == null)
                {
                    CreateCropArea();
                }

                var elementSize = Children[0].RenderSize;
                var pointBoundaries = new PointBoundaries(new Point(elementSize.Width, elementSize.Height));

                var width = Math.Abs(_mouseDownPoint.X - currentPoint.X);
                var height = Math.Abs(_mouseDownPoint.Y - currentPoint.Y);
                var left = pointBoundaries.GetXPointWithinBoundary(Math.Min(_mouseDownPoint.X,
                                                                             currentPoint.X));
                var top = pointBoundaries.GetXPointWithinBoundary(Math.Min(_mouseDownPoint.Y,
                                                                            currentPoint.Y));

                width = width < 0 ? 0 : width;
                width = width + left > elementSize.Width //need to add left with width to make sure it doesn't go out of bounds
                        ? elementSize.Width - left // when out of bounds need to subtract left
                        : width;
                height = height < 0 ? 0 : height;
                height = height + top > elementSize.Height //need to add top with height to make sure it doesn't go out of bounds
                        ? elementSize.Height - top // when out of bounds need to subtract top
                        : height;

                CropArea = new Int32Rect()
                {
                    X = Convert.ToInt32(left),
                    Y = Convert.ToInt32(top),
                    Width = Convert.ToInt32(width),
                    Height = Convert.ToInt32(height)
                };
                
                _cropShape.Width = width;
                _cropShape.Height = height;
                Canvas.SetLeft(_cropShape, left);
                Canvas.SetTop(_cropShape, top);

                //System.Diagnostics.Debug.WriteLine($"Crop Area{{X={CropArea.Value.X},Y={CropArea.Value.Y},Width={CropArea.Value.Width},Height={CropArea.Value.Height} }}");
                //System.Diagnostics.Debug.WriteLine($"Canvas{{X={Canvas.GetLeft(_cropShape)},Y={Canvas.GetTop(_cropShape)} }}, CropShape{{Width={_cropShape.Width},Height={_cropShape.Height} }}");
                //System.Diagnostics.Debug.WriteLine("");
                //System.Diagnostics.Debug.WriteLine("");
            }
        }

        private void CreateCropArea()
        {
            _cropShape = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.White) { Opacity = .10 }
            };
            Children.Add(_cropShape);
        }

        private Point _mouseDownPoint;
        private Shape _cropShape;
    }
}
