using System;
using System.Diagnostics;
using MetroFramework.Controls;

namespace SkypeVoiceChanger.Views
{
    public partial class RecordingPage : MetroUserControl
    {
        public RecordingPage()
        {
            InitializeComponent();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.markheath.net/skypevoicechanger");
        }
    }
}
