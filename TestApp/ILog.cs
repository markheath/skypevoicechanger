using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace SkypeFx
{
    interface ILog
    {
        void Info(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
    }

    class DebugLogger : ILog
    {
        public void Info(string format, params object[] args)
        {
            Debug.Write("INFO: ");
            Debug.WriteLine(string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            Debug.Write("WARN: ");
            Debug.WriteLine(string.Format(format, args));
        }


        public void Error(string format, params object[] args)
        {
            Debug.Write("ERROR: ");
            Debug.WriteLine(string.Format(format, args));
        }
    }

    class RichTextLogger : ILog
    {
        RichTextBox richTextBox;

        public RichTextLogger(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        delegate void LogMethod(string format, params object[] args);

        public void Info(string format, params object[] args)
        {
            LogMessage(Color.Blue, format, args);
        }

        public void Error(string format, params object[] args)
        {
            LogMessage(Color.Red, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            LogMessage(Color.Orange, format, args);
        }

        private void LogMessage(Color textColor, string format, object[] args)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.BeginInvoke(new LogMethod(Info), format, args);
            }
            else
            {
                richTextBox.ForeColor = textColor;
                richTextBox.AppendText(string.Format("{0:HH:MM:ss} ", DateTime.Now));
                richTextBox.AppendText(string.Format(format, args));
                richTextBox.AppendText(Environment.NewLine);
            }
        }


    }
}
