using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineEntity
{
    public class LinePainter : IPaintBusiness
    {
        public double _rotateAngle { get ; set ; } = 0;

        public UIElement Draw(IShapeEntity shape, int Thickness, SolidColorBrush Brush,
            DoubleCollection StrokeDash, SolidColorBrush fill)
        {
            var line = shape as LineEntity;

            var element = new Line()
            {
                X1 = line.Start.X,
                Y1 = line.Start.Y,
                X2 = line.End.X,
                Y2 = line.End.Y,
                StrokeThickness = Thickness,
                Stroke = Brush,
                StrokeDashArray = StrokeDash,
                Fill = fill
            };

            var width = Math.Abs(line.Start.X - line.End.X);
            var height = Math.Abs(line.Start.Y - line.End.Y);

            element.RenderTransform = new RotateTransform(_rotateAngle);
            return element;
        }

    }
}
