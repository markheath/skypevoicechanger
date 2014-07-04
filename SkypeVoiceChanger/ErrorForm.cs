using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkypeVoiceChanger
{
    public partial class ErrorForm : Form
    {
        public static void ShowErrorForm(Exception e)
        {
            var f = new ErrorForm(e);
            f.ShowDialog();
        }

        public ErrorForm(Exception e)
        {
            InitializeComponent();
            textBoxErrorDetails.Text = e.ToString();
        }

        private void OnButtonOkClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void OnReportErrorLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://skypefx.codeplex.com/discussions");
        }
    }
}
