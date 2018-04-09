using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;

namespace ImportReport
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window
    {
        public Chart(List<Statistics> statistics_list1)
        {
            InitializeComponent();



            for (int i = 0; i < statistics_list1.Count; i++)
            {
                KeyValuePair<string, int>[] pair = new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("Error Count", statistics_list1[i].ErrorCount), new KeyValuePair<string, int>("Warning Count", statistics_list1[i].WarningCount), };

                ColumnSeries series = new ColumnSeries();
                series.Title = statistics_list1[i].Circuit;
                series.DependentValuePath = "Value";
                series.IndependentValuePath = "Key";
                series.ItemsSource = pair;

                Chart1.Series.Add(series);

            }

        }
    }
}
