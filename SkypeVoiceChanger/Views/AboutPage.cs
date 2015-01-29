using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SkypeVoiceChanger
{
    public partial class AboutPage : UserControl
    {       
        public AboutPage()
        {
            InitializeComponent();
            textBoxCredits.Text =
                "With thanks to...\r\n" +
                "NAudio (http://naudio.codeplex.com)\r\n" +
            "ModernUI (http://viperneo.github.io/winforms-modernui/)\r\n" +
            "Scott Stillwell (http://www.stillwellaudio.com/)\r\n" +
            "Cockos REAPER (http://www.reaper.fm/)\r\n" +
            "Min Tran (http://min.frexy.com/)";

            var ver = GetType().Assembly.GetName().Version;
            metroLabelTitle.Text = String.Format("Skype Voice Changer {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            Process.Start("http://skypevoicechanger.net");
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Process.Start("http://skypevoicechanger.net");
        }
    }
}
