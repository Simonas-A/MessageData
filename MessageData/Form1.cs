using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace MessageData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        List<string> deletedMessages = new List<string>();
        

        public int totalMessages, maxDayMessages = 0, mostReacts = 0, totalReacts = 0, totalPolls = 0, totalImages = 0;
        int streakI = 0, streakN = 0;


        bool darkMode = true;

        Color[] ColorScheme = new Color[] { Color.FromArgb(33, 33, 33), Color.FromArgb(200, 200, 200) }; 


        List<Message> Messages = new List<Message>();

        Dictionary<string, Tuple<int,int>> Users = new Dictionary<string, Tuple<int, int>>();
        Dictionary<string, int> UserReacts = new Dictionary<string, int>();
        Dictionary<int, int> DailyMessages = new Dictionary<int, int>();

        Dictionary<string, int> ReactCount = new Dictionary<string, int>();

        Dictionary<int, int> DayMessageRate = new Dictionary<int, int>();
        Dictionary<DayOfWeek, int> WeekMessageRate = new Dictionary<DayOfWeek, int>();

        //string fullPath = "C:/Users/Simuxxl/Desktop/CombinedChatLog.txt";
        string fullPath = "G:\\Media\\Chatlog1\\messages\\inbox\\Aktyvus_yt5i4soerg\\message_1.html";
        bool defaultPath = true;

        bool multipleFiles = false;

        DateTime rootDate = new DateTime(2016, 8, 23, 12, 0, 0);

        private void Form1_Load(object sender, EventArgs e)
        {
            //openFileDialog1.FileNames = new string[] { "oba", "asd" };


            //MessageBox.Show(HtmlAgilityPack.htmlD"&quot;"); ;


            openFileDialog1.Filter = "Html files (*.html)|*.html";
            //rootDate = DateTime.Now;
            //MessageBox.Show(Math.Ceiling((new DateTime(2016, 8, 24, 0, 0, 1) - rootDate).TotalDays).ToString());
            //MessageBox.Show(rootDate.ToString());

            if (defaultPath)
            {
                button2.Enabled = true;
                label1.Text = fullPath;
            }

            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(new TabPage("User messages"));
            tabControl1.TabPages.Add(new TabPage("User reacts"));
            tabControl1.TabPages.Add(new TabPage("Statistics"));
            tabControl1.TabPages.Add(new TabPage("Reaction Statistics"));

            tabControl2.TabPages.Clear();
            tabControl2.TabPages.Add(new TabPage("General activity"));
            tabControl2.TabPages.Add(new TabPage("Weekly activity"));
            tabControl2.TabPages.Add(new TabPage("Daily activity"));
            tabControl2.TabPages.Add(new TabPage("Overall messages"));


            if (darkMode)
            {
                this.BackColor = ColorScheme[0];
                label1.ForeColor = ColorScheme[1];
                label2.ForeColor = ColorScheme[1];
                checkBox1.ForeColor = ColorScheme[1];
                checkBox2.ForeColor = ColorScheme[1];
                checkBox3.ForeColor = ColorScheme[1];
                checkBox4.ForeColor = ColorScheme[1];

                tabControl1.TabPages[0].BackColor = ColorScheme[0];
                tabControl1.TabPages[0].ForeColor = ColorScheme[1];
                tabControl1.TabPages[1].BackColor = ColorScheme[0];
                tabControl1.TabPages[1].ForeColor = ColorScheme[1];
                tabControl1.TabPages[2].BackColor = ColorScheme[0];
                tabControl1.TabPages[2].ForeColor = ColorScheme[1];
                tabControl1.TabPages[3].BackColor = ColorScheme[0];
                tabControl1.TabPages[3].ForeColor = ColorScheme[1];


                tabControl2.TabPages[0].BackColor = ColorScheme[0];
                tabControl2.TabPages[0].ForeColor = ColorScheme[1];
                tabControl2.TabPages[1].BackColor = ColorScheme[0];
                tabControl2.TabPages[1].ForeColor = ColorScheme[1];
                tabControl2.TabPages[2].BackColor = ColorScheme[0];
                tabControl2.TabPages[2].ForeColor = ColorScheme[1];
                tabControl2.TabPages[3].BackColor = ColorScheme[0];
                tabControl2.TabPages[3].ForeColor = ColorScheme[1];

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {

                multipleFiles = openFileDialog1.FileNames.Length > 1;
                
                fullPath = openFileDialog1.FileNames[0];
                label1.Text = fullPath;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetForm();

            DateTime dt0 = DateTime.Now;
            if (checkBox3.Checked)
            {
                ReadLogClip(Clipboard.GetText());
            }
            else
            {
                //ReadLog(fullPath);

                if (Path.GetExtension(fullPath) == ".html")
                {
                    if (multipleFiles)
                    {
                        foreach (string path in openFileDialog1.FileNames)
                        {
                            ReadLogHtml(path);
                        }
                    }
                    else
                    {
                        ReadLogHtml(fullPath);
                    }


                    
                }
                else if (Path.GetExtension(fullPath) == ".txt")
                {
                    ReadLog(fullPath);
                }
            }

            DateTime dt1 = DateTime.Now;
            DataManip();
            DateTime dt2 = DateTime.Now;
            FillTable();
            DateTime dt3 = DateTime.Now;


            label2.Text = "Reading: " + (dt1 - dt0).ToString() + '\n' + "Calculating: " + (dt2 - dt1).ToString() + '\n' + "Graphing: " + (dt3 - dt2).ToString();
            //button2.Enabled = false;
            //button1.Enabled = false;
        }

        private void ResetForm()
        {
            Messages.Clear();

            totalMessages = 0;
            maxDayMessages = 0;
            mostReacts = 0;
            totalReacts = 0;
            totalPolls = 0;
            totalImages = 0;
            streakI = 0;
            streakN = 0;

            Users.Clear();
            UserReacts.Clear();
            DailyMessages.Clear();
            ReactCount.Clear();
            DayMessageRate.Clear();
            WeekMessageRate.Clear();

            foreach (TabPage page in tabControl1.TabPages)
            {
                page.Controls.Clear();
            }

            foreach (TabPage page in tabControl2.TabPages)
            {
                page.Controls.Clear();
            }
        }

        private void listView_Click(object sender, EventArgs e)
        {
            //var firstSelectedItem = listView.SelectedItems[0];
        }

        private void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (tabControl1.TabPages[0].Controls.Count > 0)
            {
                ListView listView = tabControl1.TabPages[0].Controls[0] as ListView;

                if (listView.SelectedItems.Count == 1)
                {
                    UserForm form = new UserForm();
                    form.Text = listView.SelectedItems[0].Text;
                    form.darkMode = darkMode;
                    form.colorsEnabled = checkBox1.Checked;
                    string user = listView.SelectedItems[0].Text;
                    foreach (Message msg in Messages.Where(k => k.Sender == user))
                    {
                        form.UserMessages.Add(msg);
                    }
                    if (UserReacts.ContainsKey(user))
                    {
                        form.totalReacts = UserReacts[user];
                    }
                    form.ShowDialog();
                }
            }

        }

        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView listView = tabControl1.TabPages[0].Controls.OfType<ListView>().First();
            listView.Items.Clear();
            foreach (var user in Users.OrderByDescending(k => e.Column == 2 ? k.Value.Item2 : k.Value.Item1))
            {
                ListViewItem item = new ListViewItem(user.Key);
                item.SubItems.Add(user.Value.Item1.ToString());
                item.SubItems.Add(user.Value.Item2.ToString());
                listView.Items.Add(item);
            }
        }

        private void FillTable()
        {
            
            foreach(var msg in Messages.OrderByDescending(k => k.Reacts.Length))
            {
                //MessageBox.Show(msg.Date + Environment.NewLine + msg.Text + Environment.NewLine + msg.Reacts.Length.ToString() + " Reactions" + Environment.NewLine + msg.Sender);
            }

            Chart chart3 = new Chart();
            chart3.ChartAreas.Add("chartArea");
            chart3.ChartAreas.Add("chartArea1");


            Series series3 = new Series();
            series3.ChartType = SeriesChartType.Pie;
            series3.ChartArea = "chartArea";

            Series series30 = new Series();
            series30.ChartType = SeriesChartType.Pie;
            series30.ChartArea = "chartArea1";
            series30.IsVisibleInLegend = false;


            chart3.Series.Add(series3);
            chart3.Series.Add(series30);
            chart3.Size = new Size(850, 480);
            chart3.Legends.Add(new Legend());
            

            chart3.ChartAreas[0].Position = new ElementPosition(2, 5, 48, 80);
            chart3.ChartAreas[1].Position = new ElementPosition(50, 5, 48, 80);

            chart3.Legends[0].Alignment = StringAlignment.Center;
            chart3.Legends[0].Docking = Docking.Bottom;



            ListView listView = new ListView();
            listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_ItemSelectionChanged);
            listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListView_ColumnClick);
            listView.Size = new Size(460, 400);
            listView.View = View.Details;
            listView.Columns.Add("User", 200);
            listView.Columns.Add("Message count", 120);
            listView.Columns.Add("Characters count", 120);
            foreach (var user in Users.OrderByDescending(k => k.Value.Item1))
            {
                DataPoint point = new DataPoint();
                point.SetValueXY("", user.Value.Item1);
                point.ToolTip = string.Format("{0}: {1}", user.Key, user.Value.Item1);
                point.LegendText = user.Key;
                chart3.Series[0].Points.Add(point);


                DataPoint point0 = new DataPoint();
                point0.SetValueXY("", user.Value.Item2);
                point0.ToolTip = string.Format("{0}: {1}", user.Key, user.Value.Item2);
                //point0.LegendText = user.Key;
                chart3.Series[1].Points.Add(point0);



                ListViewItem item = new ListViewItem(user.Key);
                item.SubItems.Add(user.Value.Item1.ToString());
                item.SubItems.Add(user.Value.Item2.ToString());
                listView.Items.Add(item);
            }
            tabControl1.TabPages[0].Controls.Add(listView);

            tabControl2.TabPages[3].Controls.Add(chart3);

            ListView listView1 = new ListView();
            listView1.Size = new Size(460, 400);
            listView1.View = View.Details;
            listView1.Columns.Add("User", 300);
            listView1.Columns.Add("React count", 120);
            foreach (var user in UserReacts.OrderByDescending(k => k.Value))
            {
                ListViewItem item = new ListViewItem(user.Key);
                item.SubItems.Add(user.Value.ToString());
                listView1.Items.Add(item);
            }
            tabControl1.TabPages[1].Controls.Add(listView1);


            ListView listView2 = new ListView();
            listView2.Size = new Size(460, 400);
            listView2.View = View.Details;

            listView2.Columns.Add("", 300);
            listView2.Columns.Add("", 120);

            ListViewItem item0 = new ListViewItem("Total messages");
            ListViewItem item1 = new ListViewItem("Total reacts");
            ListViewItem item2 = new ListViewItem("Most messages per day");
            ListViewItem item3 = new ListViewItem("Most reacts on message");
            ListViewItem item4 = new ListViewItem("Active streak");
            ListViewItem item5 = new ListViewItem("Inactive streak");
            //ListViewItem item6 = new ListViewItem("Total polls");
            //ListViewItem item7 = new ListViewItem("Total images");


            item0.SubItems.Add(totalMessages.ToString());
            item1.SubItems.Add(totalReacts.ToString());
            item2.SubItems.Add(maxDayMessages.ToString());
            item3.SubItems.Add(mostReacts.ToString());
            item4.SubItems.Add(streakI.ToString());
            item5.SubItems.Add(streakN.ToString());
            //item6.SubItems.Add(totalPolls.ToString());
            //item7.SubItems.Add(totalImages.ToString());

            listView2.Items.Add(item0);
            listView2.Items.Add(item1);
            listView2.Items.Add(item2);
            listView2.Items.Add(item3);
            listView2.Items.Add(item4);
            listView2.Items.Add(item5);
            //listView2.Items.Add(item6);
            //listView2.Items.Add(item7);

            tabControl1.TabPages[2].Controls.Add(listView2);


            ListView listView3 = new ListView();
            listView3.Size = new Size(460, 400);
            listView3.View = View.Details;
            listView3.Columns.Add("React type", 300);
            listView3.Columns.Add("React count", 120);
            foreach (var react in ReactCount.OrderByDescending(k => k.Value))
            {
                ListViewItem item = new ListViewItem(react.Key);
                item.SubItems.Add(react.Value.ToString());
                listView3.Items.Add(item);
            }
            tabControl1.TabPages[3].Controls.Add(listView3);

            /* Total messages+
         * Active streak
         * Inactive streak
         * Most messages per day
         * Most reacts on message+
         * Total Reacts+
         */



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

            int maxD = DailyMessages.Keys.Max();

            int totalMsg = 0;

            int mostDailyMessages = DailyMessages.OrderByDescending(k => k.Value).First().Value;
            int totalMsgAll = DailyMessages.Sum(k => k.Value);

            for (int i = 0; i <= maxD; i++)
            {
                
                

                if (DailyMessages.ContainsKey(i))
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), DailyMessages[i]);
                    chart.Series[0].Points.Last().ToolTip = DailyMessages[i].ToString();

                    totalMsg += DailyMessages[i];
                }
                else
                {
                    chart.Series[0].Points.AddXY(rootDate.AddDays(i), 0);
                    chart.Series[0].Points.Last().ToolTip = "0";
                }

                chart.Series[1].Points.AddXY(rootDate.AddDays(i), totalMsg * mostDailyMessages / totalMsgAll);///

                if (checkBox1.Checked)
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


            tabControl2.TabPages[0].Controls.Add(chart);

            Chart chart1 = new Chart();
            chart1.ChartAreas.Add("chartArea");
            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Column;
            series1.BorderWidth = 5;
            series1.BorderColor = Color.Gold;
            series1.Color = Color.SeaShell;
            chart1.Series.Add(series1);
            chart1.Size = new Size(850, 480);




            //chart1.ChartAreas[0].AxisX.CustomLabels[0] = new CustomLabel()


            //string[] weekNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            foreach (var msg in WeekMessageRate)
            {
                chart1.Series[0].Points.AddXY((int)msg.Key, msg.Value);
                chart1.Series[0].Points.Last().AxisLabel = msg.Key.ToString();
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }

            /*
            string[] weekNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            int startOffset = 0;
            int endOffset = 0;
            foreach (string name in weekNames)
            {
                CustomLabel weeklabel = new CustomLabel(startOffset, endOffset, name, 0, LabelMarkStyle.None);
                chart1.ChartAreas[0].AxisX.CustomLabels.Add(weeklabel);
                startOffset = startOffset + 1;
                endOffset = endOffset + 1;
            }
            */

            tabControl2.TabPages[1].Controls.Add(chart1);

            

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
            chart2.Size = new Size(850, 480);
            foreach (var msg in DayMessageRate.OrderBy(k => k.Key))
            {
                chart2.Series[0].Points.AddXY(msg.Key, msg.Value);
                chart2.Series[0].Points.Last().Color = Color.Green;
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }
            tabControl2.TabPages[2].Controls.Add(chart2);


            if (darkMode)
            {
                chart.Series[0].LabelForeColor = ColorScheme[1];

                chart3.ChartAreas[0].BackColor = ColorScheme[0];
                chart3.ChartAreas[1].BackColor = ColorScheme[0];
                chart3.BackColor = ColorScheme[0];
                chart3.Legends[0].BackColor = ColorScheme[0];
                chart3.Legends[0].ForeColor = ColorScheme[1];


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

                listView.ForeColor = ColorScheme[1];
                listView1.ForeColor = ColorScheme[1];
                listView2.ForeColor = ColorScheme[1];
                listView3.ForeColor = ColorScheme[1];

                listView.BackColor = ColorScheme[0];
                listView1.BackColor = ColorScheme[0];
                listView2.BackColor = ColorScheme[0];
                listView3.BackColor = ColorScheme[0];
            }
        }

        private void DataManip()
        {
            //File.WriteAllLines("C:\\Users\\Simuxxl\\Desktop\\scraped.txt", deletedMessages);

            totalMessages = Messages.Count();
            mostReacts = Messages.Max(k => k.Reacts.Count());


            rootDate = Messages.OrderBy(k => k.Date).First().Date;

            foreach (Message msg in Messages)
            {
                totalReacts += msg.Reacts.Count();

                

                if (Users.ContainsKey(msg.Sender))
                {
                    Users[msg.Sender] = new Tuple<int, int>(Users[msg.Sender].Item1 + 1, Users[msg.Sender].Item2 + msg.Text.Length);
                }
                else
                {
                    Users[msg.Sender] = new Tuple<int, int>(1, msg.Text.Length);
                }



                int t = Convert.ToInt32((new DateTime(msg.Date.Year, msg.Date.Month, msg.Date.Day) - new DateTime(rootDate.Year, rootDate.Month, rootDate.Day)).TotalDays);
                if (DailyMessages.ContainsKey(t))
                {
                    DailyMessages[t]++;
                }
                else
                {
                    DailyMessages.Add(t, 1);
                }

                foreach (var react in msg.Reacts)
                {
                    if (UserReacts.ContainsKey(react.Reacter))
                    {
                        UserReacts[react.Reacter]++;
                    }
                    else
                    {
                        UserReacts.Add(react.Reacter, 1);
                    }

                    if (ReactCount.ContainsKey(react.ReactType))
                    {
                        ReactCount[react.ReactType]++;
                    }
                    else
                    {
                        ReactCount.Add(react.ReactType, 1);
                    }
                }


                if (DayMessageRate.ContainsKey(msg.Date.Hour))
                {
                    DayMessageRate[msg.Date.Hour]++;
                }
                else
                {
                    DayMessageRate.Add(msg.Date.Hour, 1);
                }


                if (WeekMessageRate.ContainsKey(msg.Date.DayOfWeek))
                {
                    WeekMessageRate[msg.Date.DayOfWeek]++;
                }
                else
                {
                    WeekMessageRate.Add(msg.Date.DayOfWeek, 1);
                }

            }
            maxDayMessages = DailyMessages.Max(k => k.Value);



            DateTime eDate = DateTime.Now;
            //int maxSpan = 0;

            DateTime baseDate = Messages.Min(k => k.Date);
            DateTime sDate = baseDate;
            DateTime sMDate = baseDate;
            int MaxStreak = 0;
            int tStreak = 1;
            //sDate = baseDate;
            foreach (var msg in Messages.OrderBy(k => k.Date))
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

            //MessageBox.Show(sMDate.ToString() + Environment.NewLine + eDate.ToString());
            //MessageBox.Show(MsgCount.Count.ToString());
        }

        private void ReadLogHtml(string fullPath)
        {
            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = true;
            doc.Load(fullPath, Encoding.UTF8);
            //var node = doc.DocumentNode.SelectSingleNode("//body");
            /*
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'pam')]");
            foreach (var node in nodes)
            {

            }
            */

            foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//body").Where(k => k.GetAttributeValue("class", String.Empty) == "_5vb_ _2yq _4yic"))
            {
                foreach (HtmlNode node0 in row.SelectNodes("div").Where(k => k.GetAttributeValue("class",String.Empty) == "clearfix _ikh"))
                {
                    foreach (HtmlNode node1 in node0.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_4bl9"))
                    {
                        foreach (HtmlNode node2 in node1.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_li"))
                        {
                            foreach (HtmlNode node3 in node2.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_3a_u"))
                            {
                                foreach (HtmlNode node4 in node3.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_4t5n"))
                                {
                                    foreach (HtmlNode node5 in node4.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "pam _3-95 _2pi0 _2lej uiBoxWhite noborder"))
                                    {
                                        Message msg = new Message();
                                        bool legitMsg = true;
                                        
                                        foreach (HtmlNode node6 in node5.SelectNodes("div"))
                                        {
                                            string elementClass = node6.GetAttributeValue("class", String.Empty);
                                            if (elementClass == "_3-96 _2pio _2lek _2lel")
                                            {
                                                msg.Sender = System.Net.WebUtility.HtmlDecode(node6.InnerText);
                                            }
                                            else if (elementClass == "_3-96 _2let")
                                            {
                                                //msgInfo += node6.InnerText + '\n';//negerai, prideda ir reactus

                                                foreach (HtmlNode node7 in node6.SelectNodes("div"))
                                                {
                                                    int c = 0;
                                                    foreach (HtmlNode node8 in node7.SelectNodes("div"))
                                                    {
                                                        List<React> reacts = new List<React>();
                                                        c++;
                                                        if (c < 5)
                                                        {
                                                            if (node8.InnerText == "This poll is no longer available." ||
                                                                node8.InnerText.Contains("in the poll.") ||
                                                                node8.InnerText.Contains("points in Snake.") ||
                                                                node8.InnerText.Contains("Basketball FRVR!") ||
                                                                node8.InnerText.Contains("Master Archer.") ||
                                                                node8.InnerText.Contains("moved up the leaderboard in") ||
                                                                node8.InnerText.Contains("set a new personal best of") ||
                                                                node8.InnerText.Contains("EverWing") ||
                                                                (node8.InnerText.Contains("the group.") && !node8.InnerText.Contains("waved hello to the group.")) ||
                                                                node8.InnerText.Contains("video chat") ||
                                                                (node8.InnerText.Contains("responded") && node8.InnerText.Contains(msg.Sender.Split(' ')[0])) ||
                                                                node8.InnerText.Contains("named the plan ") ||
                                                                node8.InnerText.Contains(" started a plan.") ||
                                                                node8.InnerText.Contains("updated the plan")
                                                                )
                                                            {
                                                                //deletedMessages.Add(node8.InnerText);
                                                                legitMsg = false;
                                                                break;
                                                            }
                                                                
                                                            msg.Text += System.Net.WebUtility.HtmlDecode(node8.InnerText);
                                                        }
                                                        else
                                                        {
                                                            if (node8.SelectSingleNode("ul") != null && node8.SelectSingleNode("ul").GetAttributeValue("class", String.Empty) == "_tqp")
                                                            {
                                                                foreach (HtmlNode node9 in node8.SelectNodes("ul"))
                                                                {
                                                                    
                                                                    foreach (HtmlNode node10 in node9.SelectNodes("li"))
                                                                    {
                                                                        if (node10.InnerText[0] == '❤')
                                                                        {
                                                                            reacts.Add(new React(node10.InnerText.Substring(1), "❤"));
                                                                        }
                                                                        else
                                                                        {
                                                                            reacts.Add(new React(node10.InnerText.Substring(2), node10.InnerText.Substring(0, 2)));
                                                                        }
                                                                    }
                                                                    
                                                                }
                                                            }
                                                        }
                                                        if (reacts.Count > 0)
                                                            msg.Reacts = reacts.ToArray();
                                                        else
                                                            msg.Reacts = reacts.ToArray();
                                                    }
                                                }
                                                
                                            }
                                            else if (node6.GetAttributeValue("class", String.Empty) == "_3-94 _2lem")
                                            {
                                                msg.Date = DateTime.Parse(node6.InnerText);
                                            }
                                            else
                                            {
                                                legitMsg = false;
                                                break;
                                            }
                                            
                                        }
                                        //MessageBox.Show(msgInfo);
                                        if (legitMsg)
                                        {
                                            Messages.Add(msg);
                                        }

                                            //MessageBox.Show(node5.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_3-96 _2pio _2lek _2lel").First().InnerText + '\n' +
                                            //            node5.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_3-96 _2let").First().InnerText + '\n' +
                                            //            node5.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "_3-94 _2lem").First().InnerText + '\n' 
                                            //            );
                                        //if (node4.GetAttributeValue("div", String.Empty) == "pam _3-95 _2pi0 _2lej uiBoxWhite noborder")
                                        //{
                                            //MessageBox.Show(node4.InnerText);
                                        //}
                                    }
                                }
                            }
                        }
                    }


                    
                    
                }
                //Console.WriteLine(cust.ID + " " + cust.TimeAdded + " " + cust.DateAdded + " " + cust.Notes);
            }

            //Application.Exit();
            //Environment.Exit(1);
        }

        private void ReadLog(string fullPath)
        {
            string[] lines = File.ReadAllLines(fullPath);
            //MessageBox.Show(lines[0][0].ToString());
            int tAdd = 0;
            for (int i = 0; i < lines.Length; i += 2 + tAdd)
            {
                Message msg = new Message();
                tAdd = 0;
                msg.Sender = lines[i];
                string text = "";
                List<React> reacts = new List<React>();


                for (int j = i + 1; j < lines.Length; j++)
                {
                    if (lines[j].Length > 3)
                    {
                        if (!IsDate(lines[j]))
                        {

                            if (!IsReact(lines[j]))
                            {
                                if (text != "")
                                {
                                    text += Environment.NewLine;
                                }
                                text += lines[j];


                                if (lines[j].Contains("created a poll:"))
                                {
                                    totalPolls++;
                                }
                            }
                            else
                            {
                                //MessageBox.Show(lines[j]);
                                if (lines[j][0] == '❤')
                                {
                                    reacts.Add(new React(lines[j].Substring(1), "❤"));
                                }
                                else
                                {
                                    reacts.Add(new React(lines[j].Substring(2), lines[j].Substring(0, 2)));
                                }
                            }

                            tAdd++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        tAdd++;
                        if (text != "")
                        {
                            text += Environment.NewLine;
                        }
                        text += lines[j];


                        if (lines[j].Contains("created a poll:"))
                        {
                            totalPolls++;
                        }
                    }
                }

                msg.Text = text;
                msg.Reacts = reacts.ToArray();
                msg.Date = DateTime.Parse(lines[i + tAdd + 1]);

                Messages.Add(msg);
            }

            DateTime date = Messages.Min(k => k.Date);
            if (date < rootDate)
            {
                rootDate = date;
            }
            //MessageBox.Show(Messages.Count.ToString());
        }

        private void ReadLogClip(string AllLines)
        {
            string[] lines = AllLines.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int tAdd = 0;
            for (int i = 0; i < lines.Length; i += 2 + tAdd)
            {
                Message msg = new Message();
                tAdd = 0;
                msg.Sender = lines[i];
                string text = "";
                List<React> reacts = new List<React>();


                for (int j = i + 1; j < lines.Length; j++)
                {
                    if (lines[j].Length > 3)
                    {
                        if (!IsDate(lines[j]))
                        {

                            if (!IsReact(lines[j]))
                            {
                                if (text != "")
                                {
                                    text += Environment.NewLine;
                                }
                                text += lines[j];


                                if (lines[j].Contains("created a poll:"))
                                {
                                    totalPolls++;
                                }
                            }
                            else
                            {
                                //MessageBox.Show(lines[j]);
                                if (lines[j][0] == '❤')
                                {
                                    reacts.Add(new React(lines[j].Substring(1), "❤"));
                                }
                                else
                                {
                                    reacts.Add(new React(lines[j].Substring(2), lines[j].Substring(0, 2)));
                                }
                            }

                            tAdd++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        tAdd++;
                        if (text != "")
                        {
                            text += Environment.NewLine;
                        }
                        text += lines[j];


                        if (lines[j].Contains("created a poll:"))
                        {
                            totalPolls++;
                        }
                    }
                }

                msg.Text = text;
                msg.Reacts = reacts.ToArray();
                msg.Date = DateTime.Parse(lines[i + tAdd + 1]);

                Messages.Add(msg);
            }

            DateTime date = Messages.Min(k => k.Date);
            if (date < rootDate)
            {
                rootDate = date;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 form = new Form2();
            form.darkMode = darkMode;
            //this.Hide();
            form.ShowDialog();
            //this.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                
                string[] Files = Directory.GetFiles(fb.SelectedPath, "*.html", SearchOption.AllDirectories);
                MessageBox.Show(String.Format("Found {0} files", Files.Length));
                DateTime dt0 = DateTime.Now;
                foreach (string file in Files)
                {
                    ReadLogHtml(file);
                }
                DateTime dt1 = DateTime.Now;
                DataManip();
                DateTime dt2 = DateTime.Now;
                FillTable();
                DateTime dt3 = DateTime.Now;

                label2.Text = "Reading: " + (dt1 - dt0).ToString() + '\n' + "Calculating: " + (dt2 - dt1).ToString() + '\n' + "Graphing: " + (dt3 - dt2).ToString();

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            WordListForm wlf = new WordListForm();
            wlf.DarkMode = darkMode;
            wlf.Messages = Messages;
            wlf.ShowDialog();

        }

        private bool IsReact(string line)
        {
            string[] Reacts = {"😍","😆","😮","👍","👎","😠", "😢", "💗" };
            if ((((Reacts.Contains(line.Substring(0,2)) && IsEnglishLetter(line[2]))) || (line[0] == '❤' && IsEnglishLetter(line[1])) ) && line.Split(' ').Length - 1 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDate(string line)
        {
            string[] Months = { "Jan ", "Feb ", "Mar ", "Apr ", "May ", "Jun ", "Jul ", "Aug ", "Sep ", "Oct ", "Nov ", "Dec "};

            if (Months.Contains(line.Substring(0,4)))
            {
                if (line.EndsWith(" AM") || line.EndsWith(" PM"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            /*
            tabControl2.TabPages[0].Controls.Clear();
            //Chart chart = new Chart();
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
            chart.Size = new Size(850, 480);
            */

            /*
            Chart chart = tabControl2.TabPages[0].Controls.OfType<Chart>().First();
            chart.Series[0].Points.Clear();


            foreach (var msg in DailyMessages.OrderBy(k => k.Key))
            {
                chart.Series[0].Points.AddXY(rootDate.AddDays(msg.Key), msg.Value);

                if (checkBox1.Checked)
                {
                    if (rootDate.AddDays(msg.Key).Month < 9 && rootDate.AddDays(msg.Key).Month > 5)//summer
                        chart.Series[0].Points.Last().Color = Color.Crimson;
                    else if (rootDate.AddDays(msg.Key).Month < 12 && rootDate.AddDays(msg.Key).Month > 8)//autumn
                        chart.Series[0].Points.Last().Color = Color.Peru;
                    else if (rootDate.AddDays(msg.Key).Month < 3 || rootDate.AddDays(msg.Key).Month > 11)//winter
                        chart.Series[0].Points.Last().Color = Color.MediumTurquoise;
                    else if (rootDate.AddDays(msg.Key).Month < 6 && rootDate.AddDays(msg.Key).Month > 2)//spring
                        chart.Series[0].Points.Last().Color = Color.SpringGreen;
                }
                //chart1.Series[0].Points.AddXY(msg.Key.ToOADate(), msg.Value);
            }
            //tabControl2.TabPages[0].Controls.Add(chart);
            */

            Chart chart = tabControl2.TabPages[0].Controls.OfType<Chart>().First();
            if (checkBox1.Checked)
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
                    point.Color = Color.FromArgb(65,140,240);
                }
            }


            tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].IsValueShownAsLabel = checkBox4.Checked;
            
            tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].ChartType = checkBox2.Checked ? SeriesChartType.RangeColumn : SeriesChartType.Line;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].ChartType = checkBox2.Checked ? SeriesChartType.RangeColumn : SeriesChartType.Line;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].IsValueShownAsLabel = checkBox4.Checked;

            //MessageBox.Show(DateTime.FromOADate(tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].Points[0].XValue).ToString());
        }

    }
}
