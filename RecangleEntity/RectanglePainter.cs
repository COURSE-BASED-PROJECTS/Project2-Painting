using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RectangleEntity
{
    public class RectanglePainter : IPaintBusiness
    {
        public double _rotateAngle { get ; set ; }

        public UIElement Draw(IShapeEntity shape, int Thickness, SolidColorBrush Brush,
            DoubleCollection StrokeDash, SolidColorBrush fill)
        {
            var rectangle = shape as RectangleEntity;

            //// TODO: chú ý việc đảo lại rightbottom và topleft 
            //double width = rectangle.RightBottom.X - rectangle.TopLeft.X;
            //double height = rectangle.RightBottom.Y - rectangle.TopLeft.Y;

            //var element = new Rectangle()
            //{
            //    Width = width,
            //    Height = height,
            //    StrokeThickness = 1,
            //    Stroke = new SolidColorBrush(Colors.Red)
            //};
            //Canvas.SetLeft(element, rectangle.TopLeft.X);
            //Canvas.SetTop(element, rectangle.TopLeft.Y);

            //return element;

            var left = Math.Min(rectangle.RightBottom.X, rectangle.TopLeft.X);
            var top = Math.Min(rectangle.RightBottom.Y, rectangle.TopLeft.Y);

            var right = Math.Max(rectangle.RightBottom.X, rectangle.TopLeft.X);
            var bottom = Math.Max(rectangle.RightBottom.Y, rectangle.TopLeft.Y);

            var width = right - left;
            var height = bottom - top;

            var rect = new Rectangle()
            {
                Width = width,
                Height = height,

                StrokeThickness = Thickness,
                Stroke = Brush,
                StrokeDashArray = StrokeDash,
                Fill = fill,
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            RotateTransform transform = new RotateTransform(_rotateAngle);
            transform.CenterX = width * 1.0 / 2;
            transform.CenterY = height * 1.0 / 2;
            rect.RenderTransform = transform;

            return rect;
        }
    }
}
