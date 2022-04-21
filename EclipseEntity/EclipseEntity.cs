using IContract;
using System;
using System.Windows;
using System.Windows.Media;

namespace EclipseEntity
{
    public class EclipseEntity : IShapeEntity, ICloneable
    {
        public Point TopLeft { get; set; }
        public Point RightBottom { get; set; }

        public string Name => "eclipse";
        public string Icon => "Images/eclipse.png";

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
