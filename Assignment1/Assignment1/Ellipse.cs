using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    internal class Ellipse : Drawable
    {
        public Rectangle Rectangle;
        public float BorderWidth;
        public Color BorderColor;
        public Color FillColor;

        public Ellipse(Rectangle rectangle, Color borderColor, Color fillColor, float borderWidth = 1) :
                                base(Color.Black, "Ellipse", new Font("Arial", 12))
        {
            Rectangle = rectangle;
            BorderWidth = borderWidth;
            BorderColor = borderColor;
            FillColor = fillColor;
            TextPosition = new Point(rectangle.X + rectangle.Width / 2 - 25, rectangle.Y + rectangle.Height / 2 - 5);
        }

        public override void Draw(Graphics graphics)
        {
            Draw(graphics, Rectangle, BorderColor, FillColor, BorderWidth);
            base.Draw(graphics);
        }

        public override void Erase(Graphics graphics, Color BackgroundColor)
        {
            Erase(graphics, BackgroundColor, Rectangle, BorderWidth);
        }

        public static void Draw(Graphics graphics, Rectangle rectangle, Color borderColor, Color fillColor, float borderWidth = 1)
        {
            graphics.DrawEllipse(new Pen(borderColor, borderWidth), rectangle);
            graphics.FillEllipse(new SolidBrush(fillColor), rectangle);
        }

        public static void Erase(Graphics graphics, Color BackgroundColor, Rectangle rectangle, float borderWidth = 1)
        {
            Draw(graphics, rectangle, BackgroundColor, BackgroundColor, borderWidth);
        }
    }
}
