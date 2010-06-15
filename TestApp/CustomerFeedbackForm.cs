using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    }
}
