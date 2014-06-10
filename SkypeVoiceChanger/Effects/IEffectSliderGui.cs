using System;

namespace SkypeVoiceChanger.Effects
{
    interface IEffectSliderGui
    {
        void Initialize(Slider slider);
        event EventHandler ValueChanged;
    }
}
