using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment4
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        Bitmap bmp = new Bitmap(1000, 1000);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                BarnsleyFern(1000000);
                var graphics = CreateGraphics();
                graphics.DrawImageUnscaled(bmp, new Point(0, 0));
            }
        }

        private void BarnsleyFern(int depth)
        {
            var graphics = CreateGraphics();
            var brush = new SolidBrush(Color.Green);
            float x = 0;
            float y = 0;
            for (var i = 0; i < depth; i++)
            {
                put(x, y);
                var selector = random.Next(100);
                float nextX;
                float nextY;
                if (selector == 0)
                {
                    nextX = 0;
                    nextY = 0.16f * y;
                }
                else if (selector > 0 && selector < 8)
                {
                    nextX = -0.15f * x + 0.28f * y;
                    nextY = 0.26f * x + 0.24f * y + 0.44f;
                }
                else if (selector > 7 && selector < 15)
                {
                    nextX = 0.2f * x - 0.26f * y;
                    nextY = 0.23f * x + 0.22f * y + 1.6f;
                }
                else
                {
                    nextX = 0.85f * x + 0.04f * y;
                    nextY = -0.04f * x + 0.85f * y + 1.6f;
                }
                x = nextX;
                y = nextY;
            }
            void put(float x, float y)
            {
                //graphics.FillRectangle(brush, (int)((x + 2.1820f) * 200), 999 - (int)(y * 100), 1, 1);
                bmp.SetPixel((int)((x + 2.1820f) * 200), 999 - (int)(y * 100), Color.Green);
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bmp = new Bitmap(1000, 1000);
            BarnsleyFern(10000000);
            var graphics = e.Graphics;
            graphics.DrawImageUnscaled(bmp, new Point(0, 0));
        }
    }
}