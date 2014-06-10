using System;
using System.Windows.Forms;

namespace SkypeVoiceChanger.Effects
{
    public partial class EffectSliderPanel : UserControl, IEffectSliderGui
    {
        Slider slider;

        public EffectSliderPanel()
        {
            InitializeComponent();
            this.trackBar1.Minimum = 0;
            this.trackBar1.Maximum = 1000;
            this.trackBar1.SmallChange = 50;
            this.trackBar1.LargeChange = 100;            
            this.trackBar1.TickFrequency = 50;
            this.trackBar1.Scroll += trackBar1_Scroll;            
        }

        void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (this.slider != null)
            {
                float value = TrackBarToSlider();
                this.slider.Value = value;
                SetSliderTextBox(value);
                // put it back to show granularity
                trackBar1.Value = SliderToTrackBar();
                RaiseValueChangedEvent(e);
            }
        }

        private void SetSliderTextBox(float value)
        {
            if (slider.DiscreteValueText.Count > 0)
            {
                int index = (int)slider.Value;
                textBoxValue.Text = slider.DiscreteValueText[index];
            }
            else
            {
                textBoxValue.Text = value.ToString();
            }
        }

        private void RaiseValueChangedEvent(EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        float TrackBarToSlider()
        {
            float value = 0;

            if (trackBar1.Value == trackBar1.Maximum)
            {
                value = slider.Maximum;
            }
            else
            {
                value = slider.Minimum + (this.trackBar1.Value * (slider.Maximum - slider.Minimum) / 1000.0f);
                value -= value % slider.Increment;
            }

            return value;
        }

        int SliderToTrackBar()
        {
            return (int) (((slider.Value - slider.Minimum) / (slider.Maximum - slider.Minimum)) * 1000.0f);
        }

        public void Initialize(Slider slider)
        {
            this.slider = slider;
            this.trackBar1.Value = SliderToTrackBar();
            this.labelDescription.Text = slider.Description;
            this.trackBar1.LargeChange = (int)(trackBar1.Maximum * (slider.Increment / (slider.Maximum - slider.Minimum)));
            SetSliderTextBox(slider.Value);
        }

        public event EventHandler ValueChanged;
    }
}
