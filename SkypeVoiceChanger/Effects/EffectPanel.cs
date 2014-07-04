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
                var panel = new EffectSliderPanel();
                panel.Initialize(slider);
                panel.ValueChanged += OnSliderValueChanged;
                flowLayoutPanel1.Controls.Add(panel);
            }
        }

        void OnSliderValueChanged(object sender, EventArgs e)
        {
            effect.SliderChanged();
        }

        public void Clear()
        {
            this.effect = null;
            flowLayoutPanel1.Controls.Clear();
        }
    }
}
