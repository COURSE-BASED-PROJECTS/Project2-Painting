using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Project2_Painting.MyAdorner
{
    public class MyRectangleAdorner : Adorner
    {
        private double angle = 0.0;
        private Point transformOrigin = new Point(0, 0);
        private FrameworkElement childElement;
        private VisualCollection visualChildrens;
        public Thumb leftTop, rightTop, leftBottom, rightBottom;
        private bool dragStarted = false;
        private bool isHorizontalDrag = false;

        public MyRectangleAdorner(UIElement element) : base(element)
        {
            visualChildrens = new VisualCollection(this);
            childElement = element as FrameworkElement;
            CreateThumbPart(ref leftTop);
            leftTop.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                }
                ResizeX(hor);
                ResizeY(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref rightTop);
            rightTop.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                }
                ResizeWidth(hor);
                ResizeY(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref leftBottom);
            leftBottom.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                //System.Diagnostics.Debug.WriteLine(hor + "," + vert + "," + (Math.Abs(hor) > Math.Abs(vert)) + "," + childElement.Height + "," + childElement.Width + "," + dragStarted + "," + isHorizontalDrag);
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                }
                ResizeX(hor);
                ResizeHeight(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref rightBottom);
            rightBottom.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                }
                ResizeWidth(hor);
                ResizeHeight(vert);
                dragStarted = false;
                e.Handled = true;
            };
        }
        public void CreateThumbPart(ref Thumb cornerThumb)
        {
            cornerThumb = new Thumb { Width = 15, Height = 15, Background = Brushes.Black };
            cornerThumb.DragStarted += (object sender, DragStartedEventArgs e) => dragStarted = true;
            visualChildrens.Add(cornerThumb);
        }
        private void ResizeWidth(double e)
        {
            double deltaHorizontal = Math.Min(-e, childElement.Width - childElement.MinWidth);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + (deltaHorizontal * transformOrigin.X * (1 - Math.Cos(angle))));
            childElement.Width -= deltaHorizontal;
        }
        private void ResizeX(double e)
        {
            double deltaHorizontal = Math.Min(e, childElement.Width - childElement.MinWidth);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaHorizontal * Math.Sin(angle) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaHorizontal * Math.Cos(angle) + (transformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle))));
            childElement.Width -= deltaHorizontal;
        }
        private void ResizeHeight(double e)
        {
            double deltaVertical = Math.Min(-e, childElement.Height - childElement.MinHeight);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) - deltaVertical * transformOrigin.Y * Math.Sin(-angle));
            childElement.Height -= deltaVertical;
        }
        private void ResizeY(double e)
        {
            double deltaVertical = Math.Min(e, childElement.Height - childElement.MinHeight);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaVertical * Math.Cos(-angle) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaVertical * Math.Sin(-angle) - (transformOrigin.Y * deltaVertical * Math.Sin(-angle)));
            childElement.Height -= deltaVertical;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            double desireWidth = AdornedElement.DesiredSize.Width;
            double desireHeight = AdornedElement.DesiredSize.Height;
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;
            leftTop.Arrange(new Rect(-adornerWidth / 2 - 15, -adornerHeight / 2 - 15, adornerWidth, adornerHeight));
            rightTop.Arrange(new Rect(desireWidth - adornerWidth / 2 + 15, -adornerHeight / 2 - 15, adornerWidth, adornerHeight));
            leftBottom.Arrange(new Rect(-adornerWidth / 2 - 15, desireHeight - adornerHeight / 2 + 15, adornerWidth, adornerHeight));
            rightBottom.Arrange(new Rect(desireWidth - adornerWidth / 2 + 15, desireHeight - adornerHeight / 2 + 15, adornerWidth, adornerHeight));
            return finalSize;
        }
        protected override int VisualChildrenCount => visualChildrens.Count;
        protected override Visual GetVisualChild(int index) => visualChildrens[index];
        //protected override void OnRender(DrawingContext drawingContext) => base.OnRender(drawingContext);
    }
}
