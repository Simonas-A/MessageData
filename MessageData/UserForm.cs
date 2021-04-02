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

namespace MessageData
{
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }

        Color[] ColorScheme = new Color[] { Color.FromArgb(33, 33, 33), Color.FromArgb(200, 200, 200) };

        public bool darkMode = true;
        public bool colorsEnabled = true;

        public List<Message> UserMessages = new List<Message>();

        Dictionary<int, int> DayMessageRate = new Dictionary<int, int>();
        Dictionary<DayOfWeek, int> WeekMessageRate = new Dictionary<DayOfWeek, int>();
        Dictionary<int, int> DailyMessages = new Dictionary<int, int>();
        DateTime rootDate = new DateTime(2016, 8, 23, 12, 0, 0);

        public int totalMessages, maxDayMessages = 0, mostReacts = 0, totalReacts = 0;
        int streakI = 0, streakN = 0;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages[0].Controls.OfType<Chart>().First().Series[0].IsValueShownAsLabel = checkBox1.Checked;
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages[0].Controls.OfType<Chart>().First().Series[0].ChartType = checkBox2.Checked ? SeriesChartType.RangeColumn : SeriesChartType.Line;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Chart chart = tabControl1.TabPages[0].Controls.OfType<Chart>().First();
            if (checkBox3.Checked)
            {
                foreach (DataPoint point in chart.Series[0].Points)
                {
                    Color color = Color.DarkOrange;

                    if (DateTime.FromOADate(point.XValue).Month < 9 && DateTime.FromOADate(point.XValue).Month > 5)//summer
                        color = Color.Crimson;
                    else if (DateTime.FromOADate(point.XValue).Month < 12 && DateTime.FromOADate(point.XValue).Month > 8)//autumn
                        color = Color.Peru;
                    else if (DateTime.FromOADate(point.XValue).Month < 3 || DateTime.FromOADate(point.XValue).Month > 11)//winter
                        color = Color.MediumTurquoise;
                    else if (DateTime.FromOADate(point.XValue).Month < 6 && DateTime.FromOADate(point.XValue).Month > 2)//spring
                        color = Color.SpringGreen;


                    point.Color = color;
                }
            }
            else
            {
                foreach (DataPoint point in chart.Series[0].Points)
                {
                    point.Color = Color.FromArgb(65, 140, 240);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WordListForm wlf = new WordListForm();
            wlf.DarkMode = darkMode;
            wlf.Messages = UserMessages;
            wlf.ShowDialog();
        }

        int maxDay = 0;
        /* Total messages+
         * Active streak
         * Inactive streak
         * Most messages per day+
         * Most reacts on message+
         * Total Reacts+
         */

        private void UserForm_Load(object sender, EventArgs e)
        {
            

            listView1.Columns.Add("", 200);
            listView1.Columns.Add("", 100);

            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add("User general activity");
            tabControl1.TabPages.Add("User weekly activity");
            tabControl1.TabPages.Add("User daily activity");

            ManipData();
            FillChart();

            if (darkMode)
            {
                this.BackColor = ColorScheme[0];
                checkBox1.ForeColor = ColorScheme[1];
                checkBox2.ForeColor = ColorScheme[1];
                checkBox3.ForeColor = ColorScheme[1];

                tabControl1.TabPages[0].BackColor = ColorScheme[0];
                tabControl1.TabPages[0].ForeColor = ColorScheme[1];
                tabControl1.TabPages[1].BackColor = ColorScheme[0];
                tabControl1.TabPages[1].ForeColor = ColorScheme[1];
                tabControl1.TabPages[2].BackColor = ColorScheme[0];
                tabControl1.TabPages[2].ForeColor = ColorScheme[1];

                listView1.ForeColor = ColorScheme[1];
                listView1.BackColor = ColorScheme[0];

            }

            checkBox3.Checked = colorsEnabled;
        }

        private void ManipData()
        {
            rootDate = UserMessages.OrderBy(k => k.Date).First().Date;

            int maxTemp = 0;
            foreach (var msg in UserMessages)
            {
                if (Math.Ceiling((msg.Date - rootDate).TotalDays) != maxDay)
                {
                    if (maxTemp > maxDayMessages)
                    {
                        maxDayMessages = maxTemp;
                    }

                    maxDay = (int)Math.Ceiling((msg.Date - rootDate).TotalDays);
                    maxTemp = 1;
                }
                else
                {
                    maxTemp++;
                }


                if (msg.Reacts.Length > mostReacts)
                    mostReacts = msg.Reacts.Length;

                if (WeekMessageRate.ContainsKey(msg.Date.DayOfWeek))
                    WeekMessageRate[msg.Date.DayOfWeek]++;
                else
                    WeekMessageRate.Add(msg.Date.DayOfWeek, 1);



                if (DayMessageRate.ContainsKey(msg.Date.Hour))
                    DayMessageRate[msg.Date.Hour]++;
                else
                    DayMessageRate.Add(msg.Date.Hour, 1);



                int d = (int)Math.Ceiling((msg.Date - rootDate).TotalDays);
                if (DailyMessages.ContainsKey(d))
                    DailyMessages[d]++;
                else
                    DailyMessages.Add(d, 1);
            }
            totalMessages = UserMessages.Count();


             /*
            int maxStreak = 0;
            int tStreak = 1;
            int mx = (int)(UserMessages.Max(k => k.Date) - rootDate).TotalDays;
            for (int i = 1; i <= mx; i++)
            {   
                if (UserMessages.Any(k => k.Date.ToShortDateString() == rootDate.AddDays(i).ToShortDateString()))
                {
                    tStreak++;
                }
                else
                {
                    if (tStreak > maxStreak)
                    {
                        maxStreak = tStreak;
                    }
                    tStreak = 1;
                }
            }
            MessageBox.Show(maxStreak.ToString());
            */


            DateTime eDate = DateTime.Now;
            //int maxSpan = 0;

            DateTime baseDate = UserMessages.Min(k => k.Date);
            DateTime sDate = baseDate;
            DateTime sMDate = baseDate;
            int MaxStreak = 0;
            int tStreak = 1;
            //sDate = baseDate;
            foreach (var msg in UserMessages.OrderBy(k => k.Date))
            {
                DateTime date = msg.Date;

                int daysPassed = Convert.ToInt32((new DateTime(date.Year, date.Month, date.Day) - new DateTime(baseDate.Year, baseDate.Month, baseDate.Day)).TotalDays);


                if (daysPassed > streakN)
                {
                    streakN = daysPassed;
                }

                if (daysPassed == 1)
                {
                     tStreak++;
                     //sDate = date;
                }
                else if (daysPassed > 1)
                {
                    


                    if (tStreak > MaxStreak)
                    {
                        eDate = date;
                        MaxStreak = tStreak;
                        sMDate = sDate;
                        //tStreak = 0;
                        //sDate = date;
                    }
                    
                    //eDate = date;
                    tStreak = 1;
                    sDate = date;
                }

                if (tStreak > MaxStreak)
                {
                    eDate = date;
                    MaxStreak = tStreak;
                    sMDate = sDate;
                }


                baseDate = date;
            }
            streakI = MaxStreak;

            //MessageBox.Show(sDate.ToString() + Environment.NewLine +eDate.ToString());
            //MessageBox.Show(maxSpan.ToString());
            //MessageBox.Show(MaxStreak.ToString() + Environment.NewLine + sMDate + Environment.NewLine + eDate + Environment.NewLine + (MaxStreak == ((int)(DateTime.Parse(eDate.ToShortDateString())- DateTime.Parse(sMDate.ToShortDateString())).TotalDays) + 1)).ToString()  ;
            //Close(); //^geras debugas^
        }

        private void FillChart()
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
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 3;
            chart.Series.Add(series);
            chart.Size = new Size(1300, 550);


            int maxD = DailyMessages.Keys.Max();

            for (int i = 0; i <= maxD; i++)
            {
                if (DailyMessages.ContainsKey(i))
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), DailyMessages[i]);
                }
                else
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), 0);
                }

                if (colorsEnabled)
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


            /*
            foreach (var msg in DailyMessages)
            {
                
                chart.Series[0].Points.AddXY(rootDate.AddDays(msg.Key), msg.Value);

                if (colorsEnabled)
                {
                    if (rootDate.AddDays(msg.Key).Month < 9 && rootDate.AddDays(msg.Key).Month > 5)//summer
                        chart.Series[0].Points.Last().Color = Color.Crimson;
                    else if (rootDate.AddDays(msg.Key).Month < 12 && rootDate.AddDays(msg.Key).Month > 8)//autumn
                        chart.Series[0].Points.Last().Color = Color.Gold;
                    else if (rootDate.AddDays(msg.Key).Month < 3 || rootDate.AddDays(msg.Key).Month > 11)//winter
                        chart.Series[0].Points.Last().Color = Color.MediumTurquoise;
                    else if (rootDate.AddDays(msg.Key).Month < 6 && rootDate.AddDays(msg.Key).Month > 2)//spring
                        chart.Series[0].Points.Last().Color = Color.SpringGreen;
                }
                
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }
            */


            tabControl1.TabPages[0].Controls.Add(chart);

            Chart chart1 = new Chart();
            chart1.ChartAreas.Add("chartArea");
            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Column;
            series1.BorderWidth = 3;
            series1.BorderColor = Color.Green;
            chart1.Series.Add(series1);
            chart1.Size = new Size(1300, 550);
            foreach (var msg in WeekMessageRate)
            {
                chart1.Series[0].Points.AddXY((int)msg.Key, msg.Value);
                chart1.Series[0].Points.Last().AxisLabel = msg.Key.ToString();
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }
            tabControl1.TabPages[1].Controls.Add(chart1);

            Chart chart2 = new Chart();
            chart2.ChartAreas.Add("chartArea");
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.MajorGrid.Interval = 4;
            chart2.ChartAreas[0].AxisX.Interval = 1;
            Series series2 = new Series();
            //series2.IsValueShownAsLabel = true;
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 3;
            series2.BorderColor = Color.Red;
            chart2.Series.Add(series2);
            chart2.Size = new Size(1300, 550);
            foreach (var msg in DayMessageRate.OrderBy(k => k.Key))
            {
                chart2.Series[0].Points.AddXY(msg.Key, msg.Value);
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }
            tabControl1.TabPages[2].Controls.Add(chart2);


            //ListViewItem item = new ListViewItem("Total messages");
            listView1.Items.Add("Total messages");
            listView1.Items.Add("Most messages per day");
            listView1.Items.Add("Most reacts on message");
            listView1.Items.Add("Total reacts");
            listView1.Items.Add("Active streak");
            listView1.Items.Add("Passive streak");

            listView1.Items[0].SubItems.Add(totalMessages.ToString());
            listView1.Items[1].SubItems.Add(maxDayMessages.ToString());
            listView1.Items[2].SubItems.Add(mostReacts.ToString());
            listView1.Items[3].SubItems.Add(totalReacts.ToString());
            listView1.Items[4].SubItems.Add(streakI.ToString());
            listView1.Items[5].SubItems.Add(streakN.ToString());


            if (darkMode)
            {
                chart2.BackColor = ColorScheme[0];
                chart2.ForeColor = ColorScheme[0];
                chart2.ChartAreas[0].BackColor = ColorScheme[0];
                chart2.ChartAreas[0].AxisX.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisY.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisX.MajorTickMark.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisY.MajorTickMark.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisX.LabelStyle.ForeColor = ColorScheme[1];
                chart2.ChartAreas[0].AxisY.LabelStyle.ForeColor = ColorScheme[1];

                chart1.BackColor = ColorScheme[0];
                chart1.ForeColor = ColorScheme[0];
                chart1.ChartAreas[0].BackColor = ColorScheme[0];
                chart1.ChartAreas[0].AxisX.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisY.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisX.MajorTickMark.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisY.MajorTickMark.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = ColorScheme[1];
                chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = ColorScheme[1];
                chart1.Series[0].Color = Color.Green;
                chart1.Series[0].BorderColor = Color.DarkGreen;


                chart.BackColor = ColorScheme[0];
                chart.ForeColor = ColorScheme[0];
                chart.ChartAreas[0].BackColor = ColorScheme[0];
                chart.ChartAreas[0].AxisX.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisY.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorScheme[1];
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = ColorScheme[1];
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = ColorScheme[1];

                chart.Series[0].LabelForeColor = ColorScheme[1];
            }


            /* Total messages
             * Active streak\
             * Inactive streak\
             * Most messages per day
             * Most reacts on message
             * Total Reacts
             */

        }
    }
}
