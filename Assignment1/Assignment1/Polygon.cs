using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    internal class Polygon : Drawable
    {
        public List<Point> Vertices;
        public Color BorderColor;
        public Color FillColor;
        public int BorderWidth;
        public Polygon(List<Point> vertices, Color borderColor, Color fillColor, int borderWidth = 1) :
                                base(Color.Black, "Polygon", new Font("Arial", 12))
        {
            Vertices = vertices;
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
            Point center = Point.Empty;
            foreach (var p in vertices)
            {
                center = new Point(center.X + p.X, center.Y + p.Y);
            }
            center = new Point(center.X / Vertices.Count, center.Y / vertices.Count);
            TextPosition = new Point(center.X - 30, center.Y);
        }

        public override void Draw(Graphics graphics)
        {
            Draw(graphics, Vertices, BorderColor, FillColor, BorderWidth);
            base.Draw(graphics);
        }

        public override void Erase(Graphics graphics, Color BackgroundColor)
        {
            Erase(graphics, BackgroundColor, Vertices, BorderWidth);
        }

        public static void Draw(Graphics graphics, List<Point> vertices, Color borderColor, Color fillColor, int borderWidth = 1)
        {
            graphics.DrawPolygon(new Pen(borderColor, borderWidth), vertices.ToArray());
            graphics.FillPolygon(new SolidBrush(fillColor), vertices.ToArray());
        }

        public static void Erase(Graphics graphics, Color BackgroundColor, List<Point> vertices, int borderWidth = 1)
        {
            Draw(graphics, vertices, BackgroundColor, BackgroundColor, borderWidth);
        }
    }
}
