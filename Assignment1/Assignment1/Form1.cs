using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Assignment1
{
    public partial class Form1 : Form
    {
        private List<Drawable> drawableList;
        public Form1()
        {
            InitializeComponent();
            drawableList = new List<Drawable>();
            drawableList.Add(new Line(new Point(100, 300), new Point(700, 300), 5, Color.Black, LineCap.RoundAnchor, LineCap.ArrowAnchor, DashStyle.Dash));
            drawableList.Add(new Ellipse(new Rectangle(600, 25, 200, 100), Color.Red, Color.Blue, 5));
            drawableList.Add(new Circle(new Point(100, 200), 50, Color.Red, Color.Blue, 5));
            drawableList.Add(new Polygon(new List<Point>{new Point(350, 350), new Point(500, 320), new Point(550, 550), new Point(300, 500) }, Color.Blue, Color.Red, 5));
            drawableList.Add(new RegularPolygon(new Point(50, 55), 5, 50, 0, Color.Blue, Color.Red, 5));
            drawableList.Add(new RegularPolygon(new Point(180, 55), 7, 50, -90, Color.Blue, Color.Red, 5));
        }

        private void UpdateLoopTimer_Tick(object sender, EventArgs e)
        {
            
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            var graphics = pictureBox1.CreateGraphics();
            foreach (var drawable in drawableList)
            {
                drawable.Draw(graphics);
            }
        }

        private void EraseButton_Click(object sender, EventArgs e)
        {
            var graphics = pictureBox1.CreateGraphics();
            foreach (var drawable in drawableList)
            {
                drawable.Erase(graphics, pictureBox1.BackColor);
            }
        }

        private void BorderColorButton_Click(object sender, EventArgs e)
        {
            if (BorderColorDialog.ShowDialog() == DialogResult.OK)
            {
                BorderColorButton.BackColor = BorderColorDialog.Color;
            }
        }

        private void FillColorButton_Click(object sender, EventArgs e)
        {
            if (FillColorDialog.ShowDialog() == DialogResult.OK)
            {
                FillColorButton.BackColor = FillColorDialog.Color;
            }
        }

        private void AddCircleButton_Click(object sender, EventArgs e)
        {
            var circle = new Circle(new Point((int)NumericX.Value, (int)NumericY.Value),
                                        (int)NumericR.Value,
                                        BorderColorDialog.Color,
                                        FillColorDialog.Color,
                                        (int)NumericWidth.Value);
            circle.Draw(pictureBox1.CreateGraphics());
            drawableList.Add(circle);
        }

        private void DrawCircleButton_Click(object sender, EventArgs e)
        {
            Circle.Draw(pictureBox1.CreateGraphics(), 
                        new Point((int)NumericX.Value, (int)NumericY.Value),
                        (int)NumericR.Value,
                        BorderColorDialog.Color,
                        FillColorDialog.Color,
                        (int)NumericWidth.Value);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            pictureBox1.CreateGraphics().Clear(pictureBox1.BackColor);
        }
    }
}