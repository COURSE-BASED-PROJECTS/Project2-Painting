using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IContract
{
    public interface IPaintBusiness
    {
        double _rotateAngle { get; set; }
        UIElement Draw(IShapeEntity entity, int Thickness, SolidColorBrush Brush,
            DoubleCollection StrokeDash, SolidColorBrush fill);
    }
}