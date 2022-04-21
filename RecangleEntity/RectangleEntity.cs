using IContract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectangleEntity
{
    public class RectangleEntity: ICloneable, IShapeEntity
    {
        public Point TopLeft { get; set; }
        public Point RightBottom { get; set; }

        public string Name => "Rectangle";
        public string Icon => "Images/rectangle.png";
        public int Thickness { get; set; }
        public SolidColorBrush Brush { get; set; }
        public SolidColorBrush Fill { get; set; }
        public DoubleCollection StrokeDash { get; set; }

        public void HandleStart(Point point)
        {
            TopLeft = point;
        }
        public void HandleEnd(Point point)
        {
            RightBottom = point;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
