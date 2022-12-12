using System.Diagnostics;

namespace UVMapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += new DragEventHandler(Form1_DragEnter);
            DragDrop += new DragEventHandler(Form1_DragDrop);
            pictureBox1.MouseDoubleClick += PB1_MouseDoubleClick;
        }

        private void PB1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            var x = (float)e.X / (float)pictureBox1.Width; 
            var y = (float)(pictureBox1.Height - e.Y) / (float)pictureBox1.Height; 
            Clipboard.SetText($"new Vector2({x.ToString("0.####").Replace(',', '.')}f, {y.ToString("0.####").Replace(',', '.')}f)");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var img = Image.FromFile(files[0]) as Bitmap;
                pictureBox1.Image = img;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}