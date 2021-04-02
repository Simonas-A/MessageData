using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageData
{
    public class Message
    {
        public string Sender;
        public string Text;
        public React[] Reacts;
        public DateTime Date;

        public Message(string sender, string text, React[] reacts, DateTime date)
        {
            Sender = sender;
            Text = text;
            Reacts = reacts;
            Date = date;
        }

        public Message()
        {
            Reacts = null;
            Text = "";
        }
    }
}
