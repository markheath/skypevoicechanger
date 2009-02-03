using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNet
{
    interface IEffectSliderGui
    {
        void Initialize(Slider slider);
        event EventHandler ValueChanged;
    }
}
