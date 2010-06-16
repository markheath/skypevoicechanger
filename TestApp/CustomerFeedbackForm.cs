using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SkypeFx
{

    public partial class CustomerFeedbackForm : Form
    {
        private Properties.Settings appSettings;

        public CustomerFeedbackForm()
        {
            InitializeComponent();

            appSettings = Properties.Settings.Default;

            rdoOptIn.Checked = appSettings.CustomerFeedbackOptIn;

            risData.Links.Add(0, risData.Text.Length, "http://www.runtimeintelligence.com/portal?CompanyID=3e35f098-ce43-4f82-9e9d-05c8b1046a45&ApplicationID=121959b2-80b3-482a-96d0-31249486bfbf");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoOptIn.Checked)
            {
                appSettings.CustomerFeedbackOptIn = true;
            }
            else if (rdoOptOut.Checked)
            {
                appSettings.CustomerFeedbackOptIn = false;
            }
            else
            {
                appSettings.CustomerFeedbackOptIn = false;
            }

            appSettings.Save();

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void risData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            
            ProcessStartInfo psi = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(psi);

        }
    }
}
