using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
namespace Assignment1
{
    internal class Line : Drawable
    {
        public Point Start;
        public Point End;
        public float Width;
        public Color Color;
        public LineCap StartCap;
        public LineCap EndCap;
        public DashStyle DashStyle;
        public Line(Point start,
                    Point end,
                    float width,
                    Color color,
                    LineCap startCap = LineCap.Flat,
                    LineCap endCap = LineCap.Flat,
                    DashStyle dashStyle = DashStyle.Solid) :
                    base(Color.Black, "Line", new Font("Arial", 12))
        {
            Start = start;
            End = end;
            Width = width;
            Color = color;
            StartCap = startCap;
            EndCap = endCap;
            DashStyle = dashStyle;
            TextPosition = new Point(start.X + ((end.X - start.X) / 2) - 20, start.Y + ((end.Y - start.Y) / 2) - 20);
        }

        public override void Draw(Graphics graphics)
        {
            Draw(graphics, Start, End, Width, Color, StartCap, EndCap, DashStyle);
            base.Draw(graphics);
        }

        public override void Erase(Graphics graphics, Color BackgroundColor)
        {
            Erase(graphics, BackgroundColor, Start, End, Width, StartCap, EndCap, DashStyle);
        }

        public static void Draw(Graphics graphics,
                    Point start,
                    Point end,
                    float width,
                    Color color,
                    LineCap startCap = LineCap.Flat,
                    LineCap endCap = LineCap.Flat,
                    DashStyle dashStyle = DashStyle.Solid)
        {
            var pen = new Pen(color, width);
            pen.StartCap = startCap;
            pen.EndCap = endCap;
            pen.DashStyle = dashStyle;
            graphics.DrawLine(pen, start, end);
        }

        public static void Erase(Graphics graphics,
                    Color BackgroundColor,
                    Point start,
                    Point end,
                    float width,
                    LineCap startCap = LineCap.Flat,
                    LineCap endCap = LineCap.Flat,
                    DashStyle dashStyle = DashStyle.Solid)
        {
            Draw(graphics, start, end, width, BackgroundColor, startCap, endCap, dashStyle);
        }
    }
}
