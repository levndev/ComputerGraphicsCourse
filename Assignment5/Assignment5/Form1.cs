namespace Assignment5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var img = Image.FromFile("tokyo.jpg") as Bitmap;
            for (var x = pictureBox1.Width / 2; x < pictureBox1.Width; x++)
            {
                for (var y = 0; y < img.Height; y++)
                {
                    var color = img.GetPixel(x, y);
                    img.SetPixel(x, y, Color.FromArgb(color.R / 2, color.G / 2, color.B / 2));
                }
            }
            pictureBox1.Image = img;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var color = (pictureBox1.Image as Bitmap).GetPixel(e.Location.X, e.Location.Y);
            button1.BackColor = color;
            var luminance = (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B).ToString("0.00");
            label1.Text = $"R:{color.R}, G:{color.G}, B:{color.B}, Luminance: {luminance}";
        }
    }
}