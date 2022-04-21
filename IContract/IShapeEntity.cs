using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IContract
{
    public interface IShapeEntity : ICloneable
    {
        string Name { get; }
        string Icon { get; }
        public int Thickness { get; set; }
        public SolidColorBrush Brush { get; set; }
        public SolidColorBrush Fill { get; set; }
        public DoubleCollection StrokeDash { get; set; }
        void HandleStart(Point point);
        void HandleEnd(Point point);
    }
}
