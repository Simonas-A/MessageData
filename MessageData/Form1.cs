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
using static MessageData.DataViewManipulator;

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

            tabControl2.TabPages.Add(new TabPage("Activity by month"));
            tabControl2.TabPages.Add(new TabPage("Activity by year"));


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

        public void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
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

        public void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
            
            //foreach(var msg in Messages.OrderByDescending(k => k.Reacts.Length))
                //MessageBox.Show(msg.Date + Environment.NewLine + msg.Text + Environment.NewLine + msg.Reacts.Length.ToString() + " Reactions" + Environment.NewLine + msg.Sender);

            int[] stats = { totalMessages, totalReacts, maxDayMessages, mostReacts, streakI, streakN };


            tabControl1.TabPages[0].Controls.Add(MessageDetailsListView(Users, ListView_ItemSelectionChanged, ListView_ColumnClick, darkMode, ColorScheme));
            tabControl1.TabPages[1].Controls.Add(UserReactsListView(UserReacts, darkMode, ColorScheme));
            tabControl1.TabPages[2].Controls.Add(ChatDetailsListView(stats, darkMode, ColorScheme));
            tabControl1.TabPages[3].Controls.Add(ReactsListView(ReactCount, darkMode, ColorScheme));

            tabControl2.TabPages[0].Controls.Add(DailyMessageChart(DailyMessages, rootDate, checkBox1.Checked, darkMode, ColorScheme));
            tabControl2.TabPages[1].Controls.Add(MessagesByWeekDayChart(WeekMessageRate, darkMode, ColorScheme));
            tabControl2.TabPages[2].Controls.Add(MessagesByHourChart(DayMessageRate, darkMode, ColorScheme));
            tabControl2.TabPages[3].Controls.Add(OverallMessagePieChart(Users, darkMode, ColorScheme));

            List<DateTime> MessageDates = Messages.Select(m => m.Date).ToList();

            //tabControl2.TabPages[4].Controls.Add(WeeklyMessageChart(MessageDates, darkMode, ColorScheme));
            tabControl2.TabPages[4].Controls.Add(MonthlyMessageChart(MessageDates, checkBox1.Checked, darkMode, ColorScheme));
            tabControl2.TabPages[5].Controls.Add(YearlyMessageChart(MessageDates, darkMode, ColorScheme));
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
            
            /*
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
            */


            //tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].IsValueShownAsLabel = checkBox4.Checked;
            
            //tabControl2.TabPages[0].Controls.OfType<Chart>().First().Series[0].ChartType = checkBox2.Checked ? SeriesChartType.RangeColumn : SeriesChartType.Line;
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
