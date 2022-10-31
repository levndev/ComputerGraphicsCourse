using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;
using org.matheval;
namespace Assignment3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Submit(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            var graphs = JsonConvert.DeserializeObject<List<Graph>>(textBox1.Text);
            foreach (var graph in graphs)
            {
                var expression = new Expression(graph.expr);
                if (expression.GetError().Count > 0)
                    continue;
                var series = new Series(graph.expr)
                {
                    ChartType = graph.chartType,
                };
                if (graph.color.HasValue)
                {
                    series.Color = graph.color.Value;
                }
                for (var x = graph.xMin; x <= graph.xMax; x += graph.xStep)
                {
                    expression.Bind("x", x);
                    series.Points.AddXY(x, expression.Eval());
                }
                chart1.Series.Add(series);
            }
        }
    }
}
