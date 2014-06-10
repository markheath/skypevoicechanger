using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace SkypeVoiceChanger
{
    public partial class AboutPage : UserControl
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.markheath.net/skypevoicechangerpro");
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.markheath.net/skypevoicechangerpro");
        }
    }
}
