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

namespace EclipseEntity
{
    public class EclipsePainter : IPaintBusiness
    {
        public double _rotateAngle { get; set; } = 0;

        public UIElement Draw(IShapeEntity entity, int Thickness, SolidColorBrush Brush,
            DoubleCollection StrokeDash, SolidColorBrush fill)
        {
            var eclipse = entity as EclipseEntity;

            var left = Math.Min(eclipse.RightBottom.X, eclipse.TopLeft.X);
            var top = Math.Min(eclipse.RightBottom.Y, eclipse.TopLeft.Y);

            var right = Math.Max(eclipse.RightBottom.X, eclipse.TopLeft.X);
            var bottom = Math.Max(eclipse.RightBottom.Y, eclipse.TopLeft.Y);

            var width = right - left;
            var height = bottom - top;

            var ellipse = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash,
                Fill = fill,
            };

            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            RotateTransform transform = new RotateTransform(_rotateAngle);
            transform.CenterX = width * 1.0 / 2;
            transform.CenterY = height * 1.0 / 2;

            ellipse.RenderTransform = transform;
            return ellipse;
        }
    }
}
