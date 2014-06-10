using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger
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
