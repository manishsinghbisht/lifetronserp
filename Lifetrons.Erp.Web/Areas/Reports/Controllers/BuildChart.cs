using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.UI.DataVisualization.Charting;

namespace Lifetrons.Erp.Controllers
{
    public class BuildChart
    {
        public BuildChart()
        {
        }

        public Chart BuildNewChart(string name, List<decimal> data)
        {
            // Build Chart
            var chart = new Chart();
            chart.Width = 350;
            chart.Height = 350;
            // Create chart here
            chart.Titles.Add(CreateTitle(name));
            // chart.Legends.Add(CreateLegend()); //Stopping legend to save space on page
            chart.ChartAreas.Add(CreateChartArea(name));

            //Create series
            chart.Series.Add(CreateSeries(name, data));

            return chart;
        }

        public string getChartImage(Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                chart.SaveImage(stream, ChartImageFormat.Png);
                string encoded = Convert.ToBase64String(stream.ToArray());
                return String.Format(img, encoded);
            }
        }

        private Title CreateTitle(string name)
        {
            Title title = new Title();
            title.Text = name;
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);
            return title;
        }

        private Legend CreateLegend()
        {
            Legend legend = new Legend();
            legend.Enabled = true;
            legend.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            legend.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            legend.ShadowOffset = 3;
            legend.ForeColor = Color.FromArgb(26, 59, 105);
            legend.Title = "Legend";
            return legend;
        }

        private ChartArea CreateChartArea(string name)
        {
            var chartArea = new ChartArea();
            chartArea.Name = name;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            return chartArea;
        }

        public Series CreateSeries(string name, List<decimal> data)
        {
            var seriesDetail = new Series();
            seriesDetail.Name = name;
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(199, 0, 0);
            seriesDetail.ChartType = SeriesChartType.Bar;
            seriesDetail.BorderWidth = 2;

            int counter = 0;
            string label = string.Empty;
            foreach (var dataPoint in data)
            {
                counter++;
                if (data.Count == 2)
                {
                    if (counter == 1)
                    {
                        label = "Current Month";
                    }
                    if (counter == 2)
                    {
                        label = "Last Month";
                    } 
                }

                if (data.Count == 3)
                {
                    if (counter == 1)
                    {
                        label = "Target";
                    }
                    if (counter == 2)
                    {
                        label = "Current Month";
                    }
                    if (counter == 3)
                    {
                        label = "Last Month";
                    }
                }
               
                //foreach (var item in s)
                //{
                var p = seriesDetail.Points.Add(Convert.ToDouble(dataPoint));
                p.Label = String.Format(label + ": {0}", dataPoint);
                p.LabelMapAreaAttributes = String.Format("href=\"javascript:void(0)\" onclick=\"myfunction('{0}')\"", 30);
                p["BarLabelStyle"] = "Center";

                //}
            }

            //for (int i = 1; i < 10; i++)
            //{
            //    var p = seriesDetail.Points.Add(i);
            //    p.Label = String.Format("Point {0}", i);
            //    p.LabelMapAreaAttributes = String.Format("href=\"javascript:void(0)\" onclick=\"myfunction('{0}')\"", i);
            //    p["BarLabelStyle"] = "Center";
            //}

            seriesDetail.ChartArea = name;
            return seriesDetail;
        }
    }
}