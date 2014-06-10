using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace SkypeVoiceChanger.Views
{
    public partial class ConnectionStatusPage : MetroUserControl
    {
        public ConnectionStatusPage()
        {
            InitializeComponent();
        }

        public ILog Log { get { return new RichTextLogger(richTextBoxLog); } }
    }
}
