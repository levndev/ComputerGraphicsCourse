using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    internal class RegularPolygon : Drawable
    {
        public Point Center;
        public int Sides;
        public double Radius;
        public double Angle;
        public Color BorderColor;
        public Color FillColor;
        public int BorderWidth;

        public RegularPolygon(Point center, int sides, int radius, double angle, Color borderColor, Color fillColor, int borderWidth = 1) :
                                base(Color.Black, "Regular Polygon", new Font("Arial", 12))
        {
            Center = center;
            Sides = sides;
            Radius = radius;
            Angle = angle;
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
            TextPosition = new Point(center.X - radius, Center.Y + radius + 20);
        }

        public override void Draw(Graphics graphics)
        {
            Draw(graphics, Center, Sides, Radius, Angle, BorderColor, FillColor, BorderWidth);
            base.Draw(graphics);
        }

        public override void Erase(Graphics graphics, Color BackgroundColor)
        {
            base.Erase(graphics, BackgroundColor);
            Erase(graphics, BackgroundColor, Center, Sides, Radius, Angle, BorderWidth);
        }

        public static void Draw(Graphics graphics, Point center, int sides, double radius, double angle, Color borderColor, Color fillColor, int borderWidth = 1)
        {
            var vertices = new List<Point>();
            for (var i = 0; i < sides; i++)
            {
                vertices.Add(GetVertexCoordinates(i, sides, center, radius, angle));
            }
            graphics.DrawPolygon(new Pen(borderColor, borderWidth), vertices.ToArray());
            graphics.FillPolygon(new SolidBrush(fillColor), vertices.ToArray());
        }

        public static void Erase(Graphics graphics, Color BackgroundColor, Point center, int sides, double radius, double angle, int borderWidth = 1)
        {
            Draw(graphics, center, sides, radius, angle, BackgroundColor, BackgroundColor, borderWidth);
        }

        private static Point GetVertexCoordinates(int index, int sides, Point center, double radius, double angle)
        {
            return new Point((int)(center.X + radius * Math.Cos((Math.PI / 180) * angle + 2 * Math.PI * index / sides)),
                             (int)(center.Y + radius * Math.Sin((Math.PI / 180) * angle + 2 * Math.PI * index / sides)));
        }
    }
}
