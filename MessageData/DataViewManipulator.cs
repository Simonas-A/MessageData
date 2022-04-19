using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MessageData
{
    internal static class DataViewManipulator
    {
        public static ListView UserMessagesListView(Dictionary<string, Tuple<int, int>> users, Action<object, ListViewItemSelectionChangedEventArgs> listView_ItemSelectionChanged, Action<object, ColumnClickEventArgs> listView_ColumnClick, bool darkMode, Color[] colorScheme)
        {

            ListView listView = new ListView();
            listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(listView_ItemSelectionChanged);
            listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(listView_ColumnClick);
            listView.Size = new Size(460, 400);
            listView.View = View.Details;
            listView.Columns.Add("User", 200);
            listView.Columns.Add("Message count", 120);
            listView.Columns.Add("Characters count", 120);
            foreach (var user in users.OrderByDescending(k => k.Value.Item1))
            {
                ListViewItem item = new ListViewItem(user.Key);
                item.SubItems.Add(user.Value.Item1.ToString());
                item.SubItems.Add(user.Value.Item2.ToString());
                listView.Items.Add(item);
            }

            if (darkMode)
            {
                listView.ForeColor = colorScheme[1];
                listView.BackColor = colorScheme[0];
            }

            return listView;
        }

        public static ListView ChatDetailsListView(int[] stats, bool darkMode, Color[] colorScheme)
        {

            ListView listView = new ListView();
            listView.Size = new Size(460, 400);
            listView.View = View.Details;

            listView.Columns.Add("", 300);
            listView.Columns.Add("", 120);

            ListViewItem item0 = new ListViewItem("Total messages");
            ListViewItem item1 = new ListViewItem("Total reacts");
            ListViewItem item2 = new ListViewItem("Most messages per day");
            ListViewItem item3 = new ListViewItem("Most reacts on message");
            ListViewItem item4 = new ListViewItem("Active streak");
            ListViewItem item5 = new ListViewItem("Inactive streak");
            //ListViewItem item6 = new ListViewItem("Total polls");
            //ListViewItem item7 = new ListViewItem("Total images");


            item0.SubItems.Add(stats[0].ToString());
            item1.SubItems.Add(stats[1].ToString());
            item2.SubItems.Add(stats[2].ToString());
            item3.SubItems.Add(stats[3].ToString());
            item4.SubItems.Add(stats[4].ToString());
            item5.SubItems.Add(stats[5].ToString());
            //item6.SubItems.Add(totalPolls.ToString());
            //item7.SubItems.Add(totalImages.ToString());

            listView.Items.Add(item0);
            listView.Items.Add(item1);
            listView.Items.Add(item2);
            listView.Items.Add(item3);
            listView.Items.Add(item4);
            listView.Items.Add(item5);
            //listView2.Items.Add(item6);
            //listView2.Items.Add(item7);

            if (darkMode)
            {
                listView.ForeColor = colorScheme[1];
                listView.BackColor = colorScheme[0];
            }

            return listView;
        }

        public static ListView UserReactsListView(Dictionary<string, int> userReacts, bool darkMode, Color[] colorScheme)
        {

            ListView listView = new ListView();
            listView.Size = new Size(460, 400);
            listView.View = View.Details;
            listView.Columns.Add("User", 300);
            listView.Columns.Add("React count", 120);
            foreach (var user in userReacts.OrderByDescending(k => k.Value))
            {
                ListViewItem item = new ListViewItem(user.Key);
                item.SubItems.Add(user.Value.ToString());
                listView.Items.Add(item);
            }

            if (darkMode)
            {
                listView.ForeColor = colorScheme[1];
                listView.BackColor = colorScheme[0];
            }

            return listView;
        }

        public static ListView ReactsListView(Dictionary<string, int> reactCount, bool darkMode, Color[] colorScheme)
        {

            ListView listView = new ListView();
            listView.Size = new Size(460, 400);
            listView.View = View.Details;
            listView.Columns.Add("React type", 300);
            listView.Columns.Add("React count", 120);
            foreach (var react in reactCount.OrderByDescending(k => k.Value))
            {
                ListViewItem item = new ListViewItem(react.Key);
                item.SubItems.Add(react.Value.ToString());
                listView.Items.Add(item);
            }

            if (darkMode)
            {
                listView.ForeColor = colorScheme[1];
                listView.BackColor = colorScheme[0];
            }

            return listView;
        }

        public static Chart OverallMessagePieChart(Dictionary<string, Tuple<int, int>> users, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            chart.ChartAreas.Add("chartArea1");


            Series series = new Series();
            series.ChartType = SeriesChartType.Pie;
            series.ChartArea = "chartArea";

            Series seriesTotal = new Series();
            seriesTotal.ChartType = SeriesChartType.Pie;
            seriesTotal.ChartArea = "chartArea1";
            seriesTotal.IsVisibleInLegend = false;


            chart.Series.Add(series);
            chart.Series.Add(seriesTotal);
            chart.Size = new Size(850, 480);
            chart.Legends.Add(new Legend());


            chart.ChartAreas[0].Position = new ElementPosition(2, 5, 48, 80);
            chart.ChartAreas[1].Position = new ElementPosition(50, 5, 48, 80);

            chart.Legends[0].Alignment = StringAlignment.Center;
            chart.Legends[0].Docking = Docking.Bottom;

            foreach (var user in users.OrderByDescending(k => k.Value.Item1))
            {
                DataPoint point = new DataPoint();
                point.SetValueXY("", user.Value.Item1);
                point.ToolTip = string.Format("{0}: {1}", user.Key, user.Value.Item1);
                point.LegendText = user.Key;
                chart.Series[0].Points.Add(point);


                DataPoint point0 = new DataPoint();
                point0.SetValueXY("", user.Value.Item2);
                point0.ToolTip = string.Format("{0}: {1}", user.Key, user.Value.Item2);
                //point0.LegendText = user.Key;
                chart.Series[1].Points.Add(point0);
            }

            if (darkMode)
            {
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[1].BackColor = colorScheme[0];
                chart.BackColor = colorScheme[0];
                chart.Legends[0].BackColor = colorScheme[0];
                chart.Legends[0].ForeColor = colorScheme[1];
            }

            return chart;
        }

        public static Chart DailyMessageChart(Dictionary<int,int> dailyMessages, DateTime rootDate, bool colorSeasons, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            //chart.ChartAreas[0].AxisX.Minimum = rootDate;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorY.AutoScroll = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd MMM yyyy";
            Series series = new Series();

            Series seriesT = new Series();

            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 3;

            seriesT.ChartType = SeriesChartType.Line;
            seriesT.BorderWidth = 3;


            chart.Series.Add(series);
            chart.Series.Add(seriesT);

            chart.Size = new Size(850, 480);

            int maxD = dailyMessages.Keys.Max();

            int totalMsg = 0;

            int mostDailyMessages = dailyMessages.OrderByDescending(k => k.Value).First().Value;
            int totalMsgAll = dailyMessages.Sum(k => k.Value);

            for (int i = 0; i <= maxD; i++)
            {



                if (dailyMessages.ContainsKey(i))
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), dailyMessages[i]);
                    chart.Series[0].Points.Last().ToolTip = dailyMessages[i].ToString();

                    totalMsg += dailyMessages[i];
                }
                else
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), 0);
                    chart.Series[0].Points.Last().ToolTip = "0";
                }

                chart.Series[1].Points.AddXY(rootDate.AddDays(i), totalMsg * mostDailyMessages / totalMsgAll);///

                if (colorSeasons)
                {
                    if (rootDate.AddDays(i).Month < 9 && rootDate.AddDays(i).Month > 5)//summer
                        chart.Series[0].Points.Last().Color = Color.Crimson;
                    else if (rootDate.AddDays(i).Month < 12 && rootDate.AddDays(i).Month > 8)//autumn
                        chart.Series[0].Points.Last().Color = Color.Peru;
                    else if (rootDate.AddDays(i).Month < 3 || rootDate.AddDays(i).Month > 11)//winter
                        chart.Series[0].Points.Last().Color = Color.MediumTurquoise;
                    else if (rootDate.AddDays(i).Month < 6 && rootDate.AddDays(i).Month > 2)//spring
                        chart.Series[0].Points.Last().Color = Color.SpringGreen;
                }
            }

            if (darkMode)
            {
                chart.Series[0].LabelForeColor = colorScheme[1];

                chart.BackColor = colorScheme[0];
                chart.ForeColor = colorScheme[0];
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = colorScheme[1];
            }

            return chart;
        }

        public static Chart MessagesByWeekDayChart(Dictionary<DayOfWeek, int> weekMessageRate, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            Series series = new Series();
            series.ChartType = SeriesChartType.Column;
            series.BorderWidth = 5;
            series.BorderColor = Color.Gold;
            series.Color = Color.SeaShell;
            chart.Series.Add(series);
            chart.Size = new Size(850, 480);


            foreach (var msg in weekMessageRate)
            {
                chart.Series[0].Points.AddXY((int)msg.Key, msg.Value);
                chart.Series[0].Points.Last().AxisLabel = msg.Key.ToString();
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }

            if (darkMode)
            {
                chart.BackColor = colorScheme[0];
                chart.ForeColor = colorScheme[0];
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = colorScheme[1];
                chart.Series[0].Color = Color.Green;
                chart.Series[0].BorderColor = Color.DarkGreen;
            }

            return chart;
        }

        public static Chart MessagesByHourChart(Dictionary<int, int> hourMessageRate, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.MajorGrid.Interval = 4;
            chart.ChartAreas[0].AxisX.Interval = 1;
            Series series = new Series();
            //series2.IsValueShownAsLabel = true;
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 3;
            series.BorderColor = Color.Red;
            chart.Series.Add(series);
            chart.Size = new Size(850, 480);
            foreach (var msg in hourMessageRate.OrderBy(k => k.Key))
            {
                chart.Series[0].Points.AddXY(msg.Key, msg.Value);
                chart.Series[0].Points.Last().Color = Color.Green;
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }




            if (darkMode)
            {
                chart.BackColor = colorScheme[0];
                chart.ForeColor = colorScheme[0];
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = colorScheme[1];
            }

            return chart;
        }

        public static Chart MonthlyMessageChart(List<DateTime> messageDates, bool colorSeasons, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            //chart.ChartAreas[0].AxisX.Minimum = rootDate;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorY.AutoScroll = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            //chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd MMM yyyy";

            Series series = new Series();
            Series seriesT = new Series();
            series.ChartType = SeriesChartType.Column;
            series.BorderWidth = 3;
            seriesT.ChartType = SeriesChartType.Line;
            seriesT.BorderWidth = 3;
            
            chart.Size = new Size(850, 480);

            int totalMsgAll = messageDates.Count;
            int mostMonthlyMessages = messageDates.GroupBy(m => new { m.Month, m.Year }).OrderByDescending(c => c.Count()).First().Count();


            DateTime oldest = messageDates.OrderBy(k => k).Select(d => new DateTime(d.Year, d.Month, 1)).First();
            DateTime latest = messageDates.OrderBy(k => k).Select(d => new DateTime(d.Year, d.Month, 1)).Last();

            Dictionary<int, int> intervalMonthly = new Dictionary<int, int>();

            for (int i = oldest.Month + oldest.Year * 12; i <= latest.Month + latest.Year * 12; i++)
            {
                intervalMonthly.Add(i, messageDates.Count(k => k.Year * 12 + k.Month == i));
            }


            int accCount = 0;

            foreach (var item in intervalMonthly.OrderBy(k => k.Key))
            {
                DataPoint dp = new DataPoint(item.Key, item.Value);

                accCount += item.Value;
                seriesT.Points.AddXY(item.Key, (int)(((UInt64)accCount * (UInt64)mostMonthlyMessages) / (UInt64)totalMsgAll));

                dp.AxisLabel = (item.Key / 12).ToString() + "-" + (item.Key % 12).ToString();

                if (colorSeasons)
                {
                    int month = item.Key % 12;

                    if (month < 9 && month > 5)//summer
                        dp.Color = Color.Crimson;
                    else if (month < 12 && month > 8)//autumn
                        dp.Color = Color.Peru;
                    else if (month < 3 || month > 11)//winter
                        dp.Color = Color.MediumTurquoise;
                    else if (month < 6 && month > 2)//spring
                        dp.Color = Color.SpringGreen;
                }

                series.Points.Add(dp);
                //series.Points.Last().AxisLabel = "";
            }

            chart.Series.Add(series);
            chart.Series.Add(seriesT);

            /*
            int maxD = dailyMessages.Keys.Max();

            int totalMsg = 0;

            int mostDailyMessages = dailyMessages.OrderByDescending(k => k.Value).First().Value;
            int totalMsgAll = dailyMessages.Sum(k => k.Value);

            for (int i = 0; i <= maxD; i++)
            {
                if (dailyMessages.ContainsKey(i))
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), dailyMessages[i]);
                    chart.Series[0].Points.Last().ToolTip = dailyMessages[i].ToString();

                    totalMsg += dailyMessages[i];
                }
                else
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), 0);
                    chart.Series[0].Points.Last().ToolTip = "0";
                }

                chart.Series[1].Points.AddXY(rootDate.AddDays(i), totalMsg * mostDailyMessages / totalMsgAll);///

                if (colorSeasons)
                {
                    if (rootDate.AddDays(i).Month < 9 && rootDate.AddDays(i).Month > 5)//summer
                        chart.Series[0].Points.Last().Color = Color.Crimson;
                    else if (rootDate.AddDays(i).Month < 12 && rootDate.AddDays(i).Month > 8)//autumn
                        chart.Series[0].Points.Last().Color = Color.Peru;
                    else if (rootDate.AddDays(i).Month < 3 || rootDate.AddDays(i).Month > 11)//winter
                        chart.Series[0].Points.Last().Color = Color.MediumTurquoise;
                    else if (rootDate.AddDays(i).Month < 6 && rootDate.AddDays(i).Month > 2)//spring
                        chart.Series[0].Points.Last().Color = Color.SpringGreen;
                }
            }

            */

            if (darkMode)
            {
                chart.Series[0].LabelForeColor = colorScheme[1];

                chart.BackColor = colorScheme[0];
                chart.ForeColor = colorScheme[0];
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = colorScheme[1];
            }

            return chart;
        }

        public static Chart YearlyMessageChart(List<DateTime> messageDates, bool darkMode, Color[] colorScheme)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add("chartArea");
            //chart.ChartAreas[0].AxisX.Minimum = rootDate;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorY.AutoScroll = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            //chart.ChartAreas[0].AxisX.LabelStyle.Format = "dd MMM yyyy";

            Series series = new Series();
            Series seriesT = new Series();
            series.ChartType = SeriesChartType.Column;
            series.BorderWidth = 3;
            seriesT.ChartType = SeriesChartType.Line;
            seriesT.BorderWidth = 3;

            chart.Size = new Size(850, 480);

            int totalMsgAll = messageDates.Count;
            int mostYearlyMessages = messageDates.GroupBy(m => m.Year).OrderByDescending(c => c.Count()).First().Count();


            DateTime oldest = messageDates.OrderBy(k => k).Select(d => new DateTime(d.Year, 1, 1)).First();
            DateTime latest = messageDates.OrderBy(k => k).Select(d => new DateTime(d.Year, 1, 1)).Last();

            Dictionary<int, int> intervalYearly = new Dictionary<int, int>();

            for (int i = oldest.Year; i <= latest.Year; i++)
            {
                intervalYearly.Add(i, messageDates.Count(k => k.Year == i));
            }


            int accCount = 0;

            foreach (var item in intervalYearly.OrderBy(k => k.Key))
            {
                DataPoint dp = new DataPoint(item.Key, item.Value);

                accCount += item.Value;
                seriesT.Points.AddXY(item.Key, (int)(((UInt64)accCount * (UInt64)mostYearlyMessages) / (UInt64)totalMsgAll));

                dp.AxisLabel = (item.Key).ToString();

                series.Points.Add(dp);
            }

            chart.Series.Add(series);
            chart.Series.Add(seriesT);

            if (darkMode)
            {
                chart.Series[0].LabelForeColor = colorScheme[1];

                chart.BackColor = colorScheme[0];
                chart.ForeColor = colorScheme[0];
                chart.ChartAreas[0].BackColor = colorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = colorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = colorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = colorScheme[1];
            }

            return chart;
        }
    }
}
