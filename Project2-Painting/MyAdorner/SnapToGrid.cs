using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project2_Painting.MyAdorner
{
    class SnapToGrid
    {
        private Size gridSizeModeCreate = new Size(13.5, 13.5);
        private Size gridSizeModeMove = new Size(13.5, 13.5);
        private double gridOffsetX = -1.0;
        private double gridOffsetY = -1.0;

        public enum SnapMode
        {
            Create,
            Move,
            Line
        }

        public Size GridSizeModeCreate
        {
            get { return gridSizeModeCreate; }
            set
            {
                gridSizeModeCreate = value;
            }
        }

        public Size GridSizeModeMove
        {
            get { return gridSizeModeMove; }
            set
            {
                gridSizeModeMove = value;
            }
        }

        public double GridOffsetX
        {
            get { return gridOffsetX; }
            set
            {
                gridOffsetX = value;
            }
        }

        public double GridOffsetY
        {
            get { return gridOffsetY; }
            set
            {
                gridOffsetY = value;
            }
        }

        private Point Calculate(Point p, Size s)
        {
            double snapX = p.X + ((Math.Round(p.X / s.Width) - p.X / s.Width) * s.Width);
            double snapY = p.Y + ((Math.Round(p.Y / s.Height) - p.Y / s.Height) * s.Height);

            return new Point(snapX + gridOffsetX, snapY + gridOffsetY);
        }

        public Point Snap(Point p, SnapMode mode)
        {
            if (mode == SnapMode.Create)
                return Calculate(p, gridSizeModeCreate);

            if (mode == SnapMode.Move)
                return Calculate(p, gridSizeModeMove);

            return new Point(0, 0);
        }
    }
}
