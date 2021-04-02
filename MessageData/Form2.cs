using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace MessageData
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public const string UserName = "Simas Albrechtas";

        string rootFolder = "G:\\Media\\Chatlog1\\messages\\inbox\\";
        public bool darkMode = true;
        //List<string> folders = new List<string>();
        Dictionary<string, List<Message>> Chats = new Dictionary<string, List<Message>>();
        List<Tuple<string, long, bool>> chatPaths = new List<Tuple<string, long, bool>>();

        Color[] ColorScheme = new Color[] { Color.FromArgb(33, 33, 33), Color.FromArgb(200, 200, 200) };

        int searchCount = 0;

        bool ignore = true;

        private void Form2_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Participant", 250);
            listView1.Columns.Add("Size", 75);

            listView2.Columns.Add("Person", 250);

            chart1.Series.Clear();
            label1.Text = rootFolder;

            if (darkMode)
            {
                BackColor = ColorScheme[0];
                listView1.BackColor = ColorScheme[0];
                listView1.ForeColor = ColorScheme[1];
                chart1.BackColor = ColorScheme[0];
                chart1.ChartAreas[0].BackColor = ColorScheme[0];

                listView2.BackColor = ColorScheme[0];
                listView2.ForeColor = ColorScheme[1];

                label1.ForeColor = ColorScheme[1];


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

                chart1.Legends[0].BackColor = ColorScheme[0];
                chart1.Legends[0].ForeColor = ColorScheme[1];

                checkBox1.ForeColor = ColorScheme[1];
                checkBox2.ForeColor = ColorScheme[1];
                checkBox3.ForeColor = ColorScheme[1];

            }

            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd MMM yyyy";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();

            if (fb.ShowDialog() == DialogResult.OK)
            {
                rootFolder = fb.SelectedPath;
            }
        }

        private void ScanFolders()
        {

            string[] directories = Directory.GetDirectories(rootFolder, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string dir in directories)
            {
                string[] files = Directory.GetFiles(dir + "\\", "*.html", SearchOption.TopDirectoryOnly);
                long size = 0;
                foreach (string file in files)
                {
                    size += new FileInfo(file).Length;
                }
                chatPaths.Add(new Tuple<string, long, bool>(dir, size, false));

                //ListViewItem item = new ListViewItem( Path.GetFileName(dir).Split('_')[0]);
                //item.SubItems.Add(size.ToString());
                //listView1.Items.Add(item);
                /*
                string[] files = Directory.GetFiles(dir + "\\", "*.html", SearchOption.TopDirectoryOnly);

                List<Message> messages = new List<Message>();
                string[] parts = { ""};
                bool privateChat = true;

                foreach (string file in files)
                {
                    messages.AddRange(ReadLogHtml(file));

                    if ((parts = Participants(messages)).Length > 2)
                    {
                        privateChat = false;
                        break;
                    }
                }

                if (privateChat && parts.Length > 1)
                {
                    if (!Chats.ContainsKey(parts[0] + ";" + parts[1]) || !Chats.ContainsKey(parts[1] + ";" + parts[0]))
                    {
                        Chats.Add(parts[0] + ";" + parts[1], messages);
                    }
                    else
                    {
                        MessageBox.Show(parts[0] + parts[1]);
                    }
                }
                */

            }

            FillList();
        }

        private string[] Participants(List<Message> messages)
        {
            List<string> participants = new List<string>();
            foreach (Message msg in messages)
            {
                if (!participants.Contains(msg.Sender))
                {
                    participants.Add(msg.Sender);
                }
            }

            return participants.ToArray();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScanFolders();
            FillList();
        }

        private void FillList()
        {
            foreach (var path in chatPaths.OrderByDescending(k => k.Item2))
            {
                ListViewItem item = new ListViewItem(Path.GetFileName(path.Item1).Split('_')[0]);
                item.SubItems.Add(path.Item2.ToString());
                item.SubItems.Add(path.Item1);
                listView1.Items.Add(item);
            }
        }

        private List<Message> ReadLogHtml(string fullPath)
        {
            List<Message> Messages = new List<Message>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(fullPath, Encoding.UTF8);
            //var node = doc.DocumentNode.SelectSingleNode("//body");

            foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//body").Where(k => k.GetAttributeValue("class", String.Empty) == "_5vb_ _2yq _4yic"))
            {
                foreach (HtmlNode node0 in row.SelectNodes("div").Where(k => k.GetAttributeValue("class", String.Empty) == "clearfix _ikh"))
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


            return Messages;
            //Application.Exit();
            //Environment.Exit(1);
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            listView2.Items.Clear();
            checkBox3.Checked = false;


            Tuple<string, long, bool> itm = chatPaths.Where(k => k.Item1 == e.Item.SubItems[2].Text).First();
            Tuple<string, long, bool> newItm = new Tuple<string, long, bool>(itm.Item1, itm.Item2, e.Item.Checked);
            chatPaths[chatPaths.IndexOf(itm)] = newItm;

            DrawChart();
        }

        private void DrawChart()
        {
            ignore = true;
            chart1.Series.Clear();

            //List<Tuple<string,bool>> listSave = new List<Tuple<string,bool>>();
            //Dictionary<string, bool> listSave = new Dictionary<string, bool>();

            /*
            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (!listSave.ContainsKey(listView2.Items[i].Text))
                {
                    listSave.Add(listView2.Items[i].Text, listView2.Items[i].Checked);
                }
                //listSave.Add(new Tuple<string, bool>(listView2.Items[i].Text, listView2.Items[i].Checked));
            }
            */

            //listView2.Items.Clear();
            foreach (var item in chatPaths.Where(k => k.Item3))
            {
                List<Message> messages = new List<Message>();

                if (!Chats.ContainsKey(item.Item1))
                {
                    foreach (string file in Directory.GetFiles(item.Item1, "*.html", SearchOption.TopDirectoryOnly))
                    {
                        messages.AddRange(ReadLogHtml(file));
                    }
                    Chats.Add(item.Item1, messages);
                }
                else
                {
                    messages = Chats[item.Item1];
                }

                if (checkBox1.Checked)
                {
                    if (checkBox3.Checked)
                    {
                        Dictionary<string, Series> series = new Dictionary<string, Series>();

                        foreach (Message msg in messages.OrderBy(k => k.Date))
                        {
                            if (series.ContainsKey(msg.Sender))
                            {
                                if (checkBox2.Checked)
                                {

                                    series[msg.Sender].Points.AddXY(msg.Date, series[msg.Sender].Points.Last().YValues[0] + msg.Text.Length);
                                }
                                else
                                {
                                    series[msg.Sender].Points.AddXY(msg.Date, series[msg.Sender].Points.Last().YValues[0] + 1);
                                }
                            }
                            else
                            {
                                
                                bool exists = false;
                                for (int i = 0; i < listView2.Items.Count; i++)
                                {
                                    if (listView2.Items[i].Text == msg.Sender)
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                

                                
                                //itm.SubItems.Add(item.Item1);
                                /*
                                if (listSave.ContainsKey(msg.Sender))
                                {
                                    itm.Checked = listSave[msg.Sender];
                                }
                                */

                                

                                if (!exists)
                                {
                                    ListViewItem itm = new ListViewItem(msg.Sender);
                                    listView2.Items.Add(itm);
                                }

                                if (checkBox2.Checked)
                                {
                                    Series ser = new Series(msg.Sender + item.Item1);
                                    ser.ChartType = SeriesChartType.Line;
                                    ser.BorderWidth = 5;
                                    ser.Points.AddXY(msg.Date, msg.Text.Length);
                                    ser.LegendText = msg.Sender + " to " +Path.GetFileName(item.Item1).Split('_')[0];

                                    series.Add(msg.Sender, ser);
                                }
                                else
                                {
                                    Series ser = new Series(msg.Sender + item.Item1);
                                    ser.ChartType = SeriesChartType.Line;
                                    ser.BorderWidth = 5;
                                    ser.Points.AddXY(msg.Date, 1);
                                    ser.LegendText = msg.Sender + " to " + Path.GetFileName(item.Item1).Split('_')[0];

                                    series.Add(msg.Sender, ser);
                                }
                            }
                        }

                        //double max = 0; 
                        foreach (var ser in series)
                        {
                            for (int i = 0; i < listView2.Items.Count; i++)
                            {
                                if (ser.Value.LegendText.Length >= listView2.Items[i].Text.Length && listView2.Items[i].Text == ser.Value.LegendText.Substring(0, listView2.Items[i].Text.Length))
                                {
                                    if (listView2.Items[i].Checked)
                                    {
                                        if (chart1.Series.FindByName(ser.Value.Name) == null)
                                        {
                                            chart1.Series.Add(ser.Value);

                                            /*
                                            if (ser.Value.Points.Last().YValues[0] > max)
                                            {
                                                max = ser.Value.Points.Last().YValues[0];
                                            }
                                            */
                                        }
                                    }
                                }
                            }  
                        }

                        /*
                        if (listView1.CheckedItems.Count == 1 || max > chart1.ChartAreas[0].AxisY.Maximum)
                            chart1.ChartAreas[0].AxisY.Maximum = max;
                        */
                    }
                    else
                    {
                        Series seriesS = new Series();
                        seriesS.ChartType = SeriesChartType.Line;
                        seriesS.BorderWidth = 5;


                        Series seriesR = new Series();
                        seriesR.ChartType = SeriesChartType.Line;
                        seriesR.BorderWidth = 5;

                        int countS = 0;
                        int countR = 0;

                        foreach (Message msg in messages.OrderBy(k => k.Date))
                        {
                            if (msg.Sender == UserName)
                            {
                                if (checkBox2.Checked)
                                {
                                    countS += msg.Text.Length;
                                }
                                else
                                {
                                    countS++;
                                }
                                seriesS.Points.AddXY(msg.Date, countS);
                            }
                            else
                            {
                                if (checkBox2.Checked)
                                {
                                    countR += msg.Text.Length;
                                }
                                else
                                {
                                    countR++;
                                }
                                seriesR.Points.AddXY(msg.Date, countR);
                            }

                        }

                        chart1.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Max(countR, countS), chart1.ChartAreas[0].AxisY.Maximum);



                        seriesS.LegendText = "Sent to " + Path.GetFileName(item.Item1).Split('_')[0];
                        seriesR.LegendText = "Received from " + Path.GetFileName(item.Item1).Split('_')[0];
                        chart1.Series.Add(seriesS);
                        chart1.Series.Add(seriesR);

                        if (chart1.Series.Count == 2)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = Math.Max(countR, countS);
                        }
                    }
                }
                else
                {
                    Series series = new Series();
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = 5;
                    int count = 0;
                    foreach (Message msg in messages.OrderBy(k => k.Date))
                    {
                        if (checkBox2.Checked)
                        {
                            count += msg.Text.Length;
                        }
                        else
                        {
                            count++;
                        }
                        series.Points.AddXY(msg.Date, count);
                    }


                    chart1.ChartAreas[0].AxisY.Maximum = Math.Max(count, chart1.ChartAreas[0].AxisY.Maximum);
                    

                    series.LegendText = Path.GetFileName(item.Item1).Split('_')[0];
                    chart1.Series.Add(series);

                    if (chart1.Series.Count == 1)
                    {
                        chart1.ChartAreas[0].AxisY.Maximum = count;
                    }
                }
                //DateTime date = messages.OrderBy(k => k.Date).First().Date;

                //DateTime lastDate = messages.OrderByDescending(k => k.Date).First().Date;
                /*
                if (date.DayOfYear != lastDate.DayOfYear || date.Year != lastDate.Year)
                {
                    foreach (Message msg in messages.OrderBy(k => k.Date))
                    {
                        if (date.DayOfYear != msg.Date.DayOfYear || date.Year != msg.Date.Year)
                        {
                            series.Points.AddXY(date, count);
                        }
                        date = msg.Date;
                        count++;

                    }
                }
                else
                {
                    foreach (Message msg in messages.OrderBy(k => k.Date))
                    {
                        if (date.Minute != msg.Date.Minute || date.Hour != msg.Date.Hour)
                        {
                            series.Points.AddXY(date, count);
                        }
                        date = msg.Date;
                        count++;
                    }
                }
                series.Points.AddXY(lastDate, count);
                */



                //series.Legend = chart1.Legends;

                //chart1.Series.Last().Legend = Path.GetFileName(item.Item1).Split('_')[0];
            }
            ignore = false;

            double max = 0;
            foreach (Series series in chart1.Series)
            {
                if (series.Points.Last().YValues[0] > max)
                {
                    max = series.Points.Last().YValues[0];
                }
            }
            chart1.ChartAreas[0].AxisY.Maximum = max;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int tries = searchCount;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                
                if (listView1.Items[i].Text.Length >= textBox1.Text.Length && listView1.Items[i].Text.ToLower().Substring(0,textBox1.Text.Length) == textBox1.Text.ToLower())
                {
                    if (tries > 0)
                    {
                        tries--;
                        listView1.Items[i].Selected = false;
                    }
                    else
                    {
                        listView1.Items[i].Selected = true;
                        listView1.Items[i].EnsureVisible();
                        break;
                    }
                }
            }
            searchCount++;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            searchCount = 0;
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            DrawChart();
            checkBox3.Enabled = checkBox1.Checked;
            if (!checkBox1.Checked)
            {
                checkBox3.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            DrawChart();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            DrawChart();
        }

        private void listView2_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!ignore)
            DrawChart();
        }
    }
}
