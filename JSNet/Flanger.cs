// This effect Copyright (C) 2004 and later Cockos Incorporated
// License: GPL - http://www.gnu.org/licenses/gpl.html
// Ported to .NET by Mark Heath
using System;
using System.ComponentModel.Composition;

namespace JSNet
{
    [Export(typeof(Effect))]    
    public class Flanger : Effect
    {
        public Flanger()
        {
            AddSlider(6, 0, 200, 1, "length (ms)");
            AddSlider(-120, -120, 6, 1, "feedback (dB)");
            AddSlider(-6, -120, 6, 1, "wet mix (dB)");
            AddSlider(-6, -120, 6, 1, "dry mix (dB)");
            AddSlider(0.6f, 0.001f, 100, 0.1f, "rate (hz)");
        }

        float odelay;
        float delaylen;
        float wetmix;
        float wetmix2;
        float drymix2;
        float dppossc;
        float dpbacksc;
        float dppos;
        float dpback;
        float dpint;
        float delaypos;
        float[] buffer = new float[500000];

        public override string Name
        {
            get { return "Flanger"; }
        }

        public override void Init()
        {
            delaypos = 0;
        }

        public override void Slider()
        {
            odelay = delaylen;
            delaylen = Math.Min(slider1 * SampleRate / 1000,500000);
            //if (odelay != delaylen) freembuf(delaylen*2);

            wetmix = pow(2, slider2 / 6);
            wetmix2 = pow(2, slider3 / 6);
            drymix2 = pow(2, slider4 / 6);
            dppossc = 2 * PI * slider5 / SampleRate;
            dpbacksc = delaylen * 0.5f - 1;
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            dppos = dppos+dppossc;
            dpback = (sin(dppos)+1)*dpbacksc;
            dpint = delaypos-dpback-1;
            if(dpint < 0) dpint += delaylen;

            dpint *= 2;

            float os1 = buffer[(int)dpint+0];
            float os2 = buffer[(int)dpint+1];

            dpint = delaypos*2;

            buffer[(int)dpint+0] = spl0 + os1*wetmix;
            buffer[(int)dpint+1] = spl1 + os2*wetmix;
            delaypos+=1;
            if(delaypos >= delaylen) delaypos=0;

            spl0=spl0*drymix2 + os1*wetmix2;
            spl1=spl1*drymix2 + os2*wetmix2;
        }
    }
}
