using System;
using System.Windows.Forms;

namespace SkypeVoiceChanger.Effects
{
    public partial class EffectPanel : UserControl, IEffectGui
    {
        private Effect effect;

        public EffectPanel()
        {
            InitializeComponent();
        }

        public void Initialize(Effect effect)
        {
            this.effect = effect;
            flowLayoutPanel1.Controls.Clear();
            foreach (Slider slider in effect.Sliders)
            {
                EffectSliderPanel panel = new EffectSliderPanel();
                panel.Initialize(slider);
                panel.ValueChanged += new EventHandler(panel_ValueChanged);
                flowLayoutPanel1.Controls.Add(panel);
            }
        }

        void panel_ValueChanged(object sender, EventArgs e)
        {
            effect.Slider();
        }

        public void Clear()
        {
            this.effect = null;
            flowLayoutPanel1.Controls.Clear();
        }
    }
}
