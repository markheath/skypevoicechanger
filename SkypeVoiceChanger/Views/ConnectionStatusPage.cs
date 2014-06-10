using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using SkypeVoiceChanger.Audio;

namespace SkypeVoiceChanger.Views
{
    public partial class ConnectionStatusPage : MetroUserControl
    {
        private SkypeStatus connectionStatus;

        public ConnectionStatusPage()
        {
            InitializeComponent();
        }

        public ILog Log { get { return new RichTextLogger(richTextBoxLog); } }

        public SkypeStatus ConnectionStatus
        {
            get { return connectionStatus; }
            set
            {
                connectionStatus = value;
                switch (value)
                {
                    // Metro colours available here: http://stackoverflow.com/a/11579376/7532
                    case SkypeStatus.PendingAuthorisation:
                        metroTile1.Text = "Pending Authorisation";
                        metroTile1.BackColor = MetroColors.Orange;
                        break;
                    case SkypeStatus.SkypeNotRunning:
                        metroTile1.Text = "Skype Not Running";
                        metroTile1.BackColor = MetroColors.Red;
                        break;
                    case SkypeStatus.WaitingForCall:
                        metroTile1.Text = "Waiting for Call";
                        metroTile1.BackColor = MetroColors.Blue;
                        break;
                    case SkypeStatus.SkypeAttachRefused:
                        metroTile1.Text = "Authorization Denied";
                        metroTile1.BackColor = MetroColors.Red;
                        break;
                    case SkypeStatus.CallInProgress:
                        metroTile1.Text = "Call in Progress";
                        metroTile1.BackColor = MetroColors.Green;
                        break;
                }
                


            }
        }
    }
}
