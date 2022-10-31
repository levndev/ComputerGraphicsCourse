using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    internal class Drawable
    {
        public Point TextPosition;
        public string Text;
        public Color TextColor;
        public Font Font;

        public Drawable(Color textColor, string text = "", Font font = null, Point textPosition = new Point())
        {
            TextPosition = textPosition;
            Text = text;
            TextColor = textColor;
            Font = font;
        }

        public virtual void Draw(Graphics graphics)
        {
            graphics.DrawString(Text, Font, new SolidBrush(TextColor), TextPosition);
        }

        public virtual void Erase(Graphics graphics, Color BackgroundColor)
        {
            graphics.DrawString(Text, Font, new SolidBrush(BackgroundColor), TextPosition);
        }
    }
}
