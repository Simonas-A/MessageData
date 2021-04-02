using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessageData
{
    public partial class WordListForm : Form
    {
        public WordListForm()
        {
            InitializeComponent();
        }

        public bool DarkMode = true;

        Color[] ColorScheme = new Color[] { Color.FromArgb(33, 33, 33), Color.FromArgb(200, 200, 200) };

        public List<Message> Messages = new List<Message>();
        public Dictionary<string, int> Dict = new Dictionary<string, int>();

        int page = 0;
        int linesPerPage = 2500;

        private void WordListForm_Load(object sender, EventArgs e)
        {
            foreach (Message msg in Messages)
            {
                //MessageBox.Show(msg.Text);
            }


            if (DarkMode)
            {
                BackColor = ColorScheme[0];
                listView1.BackColor = ColorScheme[0];
                listView1.ForeColor = ColorScheme[1];
                label1.ForeColor = ColorScheme[1];
            }

            listView1.Columns.Add("No.", 50);
            listView1.Columns.Add("Word", 250);
            listView1.Columns.Add("Frequency", 150);

            FillDictionary();
            label1.Text = "Total words: " + Dict.Count();
            FillTable();
        }

        private void FillTable()
        {
            listView1.Items.Clear();
            int i = 1 + page * linesPerPage;
            foreach (var pair in Dict.OrderByDescending(k => k.Value).Skip(page * linesPerPage).Take(linesPerPage))
            {
                ListViewItem itm = new ListViewItem(i.ToString());
                itm.SubItems.Add(pair.Key.ToString());
                itm.SubItems.Add(pair.Value.ToString());
                listView1.Items.Add(itm);
                i++;
            }
        }

        private void FillDictionary()
        {
            string punct = "\'\"\\/;:?>.<,`~!@#$%^&*()_+-={[}]| \t\n";

            foreach (Message msg in Messages)
            {
                string[] words = msg.Text.Split(punct.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    if (Dict.ContainsKey(word.ToLower()))
                    {
                        Dict[word.ToLower()]++;
                    }
                    else
                    {
                        Dict.Add(word.ToLower(), 1);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            page++;
            FillTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (page > 0)
                page--;
            FillTable();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string word = textBox1.Text.ToLower();
            if (Dict.ContainsKey(word))
            {
                MessageBox.Show(word + '\n' + "Frequency: " + Dict[word] + '\n' + "Rating: " + (Dict.OrderByDescending(k => k.Value).Where(k => k.Value > Dict[word]).Count() + 1).ToString());
            }
            else
            {
                MessageBox.Show("Word not found");
            }
        }
    }
}
