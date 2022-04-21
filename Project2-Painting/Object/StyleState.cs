using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project2_Painting.Object
{
    public class StyleState
    {
        public int Thickness { get; set; } = 1;
        public SolidColorBrush Brush { get; set; }= new SolidColorBrush(Colors.Black);
        public SolidColorBrush fill { get; set; }= null;
        public DoubleCollection StrokeDash { get; set; } = null;
    }
}
