using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project2_Painting.MyAdorner
{
    public class MyLineAdorner : Adorner
    {
        bool IsStartPoint = false;
        bool IsControlModeOn = false;
        Size size = new Size(10, 10);
        SnapToGrid snap = new SnapToGrid();

        public MyLineAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(LineAdorner_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LineAdorner_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(LineAdorner_MouseMove);
        }

        void LineAdorner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
                IsControlModeOn = true;

            Line line = this.AdornedElement as Line;
            Point p = snap.Snap(e.GetPosition(line), SnapToGrid.SnapMode.Move);

            double dStart = 0.0;
            double dEnd = 0.0;

            if (!this.IsMouseCaptured)
            {
                dStart = Math.Sqrt(Math.Pow(line.X1 - p.X, 2) + Math.Pow(line.Y1 - p.Y, 2));
                dEnd = Math.Sqrt(Math.Pow(line.X2 - p.X, 2) + Math.Pow(line.Y2 - p.Y, 2));
            }

            if (IsControlModeOn)
            {
                if (this.IsMouseCaptured)
                {
                    if (IsStartPoint)
                    {
                        line.X1 = p.X;
                        line.Y1 = p.Y;
                    }
                    else
                    {
                        line.X2 = p.X;
                        line.Y2 = p.Y;
                    }

                    this.InvalidateVisual();
                    this.ReleaseMouseCapture();

                    IsControlModeOn = false;
                }
                else
                {
                    if (dStart < dEnd)
                        IsStartPoint = true;
                    else
                        IsStartPoint = false;

                    this.InvalidateVisual();
                    this.CaptureMouse();
                }
            }
            else
            {
                if (!this.IsMouseCaptured)
                {
                    if (dStart < dEnd)
                        IsStartPoint = true;
                    else
                        IsStartPoint = false;

                    this.InvalidateVisual();
                    this.CaptureMouse();
                }
            }
        }

        void LineAdorner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsControlModeOn)
            {
                if (this.IsMouseCaptured)
                {
                    Line line = this.AdornedElement as Line;
                    Point p = snap.Snap(e.GetPosition(line), SnapToGrid.SnapMode.Move);

                    if (IsStartPoint)
                    {
                        line.X1 = p.X;
                        line.Y1 = p.Y;
                    }
                    else
                    {
                        line.X2 = p.X;
                        line.Y2 = p.Y;
                    }

                    this.InvalidateVisual();
                    this.ReleaseMouseCapture();
                }
            }
        }

        void LineAdorner_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                if (this.AdornedElement.GetType() == typeof(Line))
                {
                    Line line = this.AdornedElement as Line;
                    Point p = snap.Snap(e.GetPosition(line), SnapToGrid.SnapMode.Move);

                    // mode: move start or end point
                    if (IsStartPoint)
                    {
                        line.X1 = p.X;
                        line.Y1 = p.Y;
                    }
                    else
                    {
                        line.X2 = p.X;
                        line.Y2 = p.Y;
                    }

                    this.InvalidateVisual();
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement.GetType() == typeof(Line))
            {
                Line line = this.AdornedElement as Line;

                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                SolidColorBrush penBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                Pen pen = new Pen(penBrush, 1.0);

                Point p1 = new Point(line.X1, line.Y1);
                Point p2 = new Point(line.X2, line.Y2);

                p1.Offset(-size.Width / 2, -size.Height / 2);
                p2.Offset(-size.Width / 2, -size.Height / 2);

                Rect r1 = new Rect(p1, size);
                Rect r2 = new Rect(p2, size);

                double halfPenWidth = pen.Thickness / 2;

                GuidelineSet g1 = new GuidelineSet();
                g1.GuidelinesX.Add(r1.Left + halfPenWidth);
                g1.GuidelinesX.Add(r1.Right + halfPenWidth);
                g1.GuidelinesY.Add(r1.Top + halfPenWidth);
                g1.GuidelinesY.Add(r1.Bottom + halfPenWidth);
                drawingContext.PushGuidelineSet(g1);

                drawingContext.DrawRectangle(brush, pen, r1);
                drawingContext.Pop();

                GuidelineSet g2 = new GuidelineSet();
                g2.GuidelinesX.Add(r2.Left + halfPenWidth);
                g2.GuidelinesX.Add(r2.Right + halfPenWidth);
                g2.GuidelinesY.Add(r2.Top + halfPenWidth);
                g2.GuidelinesY.Add(r2.Bottom + halfPenWidth);
                drawingContext.PushGuidelineSet(g2);

                drawingContext.DrawRectangle(brush, pen, r2);
                drawingContext.Pop();
            }

            base.OnRender(drawingContext);
        }
    }
}
