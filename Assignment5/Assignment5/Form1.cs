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
            pictureBox1.Image = Image.FromFile("tokyo.jpg");
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var color = (pictureBox1.Image as Bitmap).GetPixel(e.Location.X, e.Location.Y);
            button1.BackColor = color;
            label1.Text = (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B).ToString("0.00");
        }
    }
}