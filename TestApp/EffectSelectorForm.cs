using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JSNet;

namespace TestApp
{
    public partial class EffectSelectorForm : Form
    {
        public EffectSelectorForm(ICollection<Effect> effects)
        {
            InitializeComponent();
            listBoxEffects.DisplayMember = "Name";
            listBoxEffects.DataSource = effects;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public Effect SelectedEffect
        {
            get
            {
                return (Effect)listBoxEffects.SelectedItem;
            }
        }

        private void listBoxEffects_DoubleClick(object sender, EventArgs e)
        {
            buttonOK_Click(sender, e);
        }
    }
}
