using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Assignment3
{
    public class Graph
    {
        public string expr;
        public int xMin = 0;
        public int xMax = 100;
        public int xStep = 1;
        public SeriesChartType chartType = SeriesChartType.Line;
        public Color? color = null;
    }
}
