using IContract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineEntity
{
    public class LineEntity : IShapeEntity, ICloneable
    {
        public Point Start { get; set; }
        public Point End { get; set; }


        public string Name => "Line";
        public string Icon => "Images/line.png";

        public int Thickness { get ; set ; }
        public SolidColorBrush Brush { get ; set ; }
        public SolidColorBrush Fill { get ; set ; }
        public DoubleCollection StrokeDash { get ; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void HandleEnd(Point point)
        {
            End = point;
        }

        public void HandleStart(Point point)
        {
            Start = point;
        }
    }
}
