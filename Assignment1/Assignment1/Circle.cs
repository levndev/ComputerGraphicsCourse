using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    internal class Circle : Drawable
    {
        public Point Center;
        public int Radius;
        public Color BorderColor;
        public Color FillColor;
        public float BorderWidth;
        public Circle(Point center, int radius, Color borderColor, Color fillColor, float borderWidth = 1) :
                                base(Color.Black, "Circle", new Font("Arial", 12))
        {
            Center = center;
            Radius = radius;
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
            TextPosition = new Point(center.X - 20, center.Y - 5);
        }

        public override void Draw(Graphics graphics)
        {
            Draw(graphics, Center, Radius, BorderColor, FillColor, BorderWidth);
            base.Draw(graphics);
        }

        public override void Erase(Graphics graphics, Color BackgroundColor)
        {
            Erase(graphics, BackgroundColor, Center, Radius, BorderWidth);
        }

        public static void Draw(Graphics graphics, Point center, int radius, Color borderColor, Color fillColor, float borderWidth = 1)
        {
            var rectangle = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.DrawEllipse(new Pen(borderColor, borderWidth), rectangle);
            graphics.FillEllipse(new SolidBrush(fillColor), rectangle);
        }

        public static void Erase(Graphics graphics, Color BackgroundColor, Point center, int radius, float borderWidth = 1)
        {
            Draw(graphics, center, radius, BackgroundColor, BackgroundColor, borderWidth);
        }
    }
}
