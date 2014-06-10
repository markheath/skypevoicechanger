// This effect Copyright (C) 2004 and later Cockos Incorporated
// License: GPL - http://www.gnu.org/licenses/gpl.html
// ported to .NET by Mark Heath
using System;
using System.ComponentModel.Composition;

namespace SkypeVoiceChanger.Effects
{
    [Export(typeof(Effect))]
    public class Delay : Effect
    {
        float delaypos;
        float odelay;
        float delaylen;
        float wetmix;
        float drymix;
        float wetmix2;
        float drymix2;
        float rspos;
        int rspos2;
        float drspos;
        int tpos;
        float[] buffer = new float[500000];

        public Delay()
        {
            AddSlider(300, 0, 4000, 20, "delay (ms)");
            AddSlider(-5, -120, 6, 1, "feedback (dB)");
            AddSlider(0, -120, 6, 1, "mix in (dB)");
            AddSlider(-6, -120, 6, 1, "output wet (dB)");
            AddSlider(0, -120, 6, 1, "output dry (dB)");
            Slider resampleSlider = AddSlider(0, 0, 1, 1, "resample on length change");
            resampleSlider.DiscreteValueText.Add("off");
            resampleSlider.DiscreteValueText.Add("on");
        }

        public override string Name
        {
            get { return "Delay"; }
        }

        public override void Init()
        {
            delaypos = 0;
        }

        public override void Slider()
        {
            odelay=delaylen;
            delaylen=Math.Min(slider1 * SampleRate / 1000,500000);
            if(odelay != delaylen) 
            {
                if(slider6 > 0 && odelay > delaylen) 
                {
                    // resample down delay buffer, heh
                    rspos=0; rspos2=0;
                    drspos=odelay/delaylen;
                    for(int n  =0; n < delaylen; n++)
                    {
                        tpos = ((int)rspos)*2;
                        buffer[rspos2+0]=buffer[tpos+0];
                        buffer[rspos2+1]=buffer[tpos+1];
                        rspos2+=2;
                        rspos+=drspos;
                    }
                    delaypos /= drspos;
                    delaypos = (int) delaypos;
                    if (delaypos<0) delaypos=0;
                } 
                else 
                {
                    if(slider6 > 0 && odelay < delaylen) 
                    {
                        // resample up delay buffer, heh
                        drspos=odelay/delaylen;
                        rspos=odelay; 
                        rspos2=(int)delaylen*2;
                        for (int n = 0; n < (int)delaylen; n++)
                        {
                           rspos-=drspos;
                           rspos2-=2;
                         
                           tpos = ((int)(rspos))*2;
                           buffer[rspos2+0]=buffer[tpos+0];
                           buffer[rspos2+1]=buffer[tpos+1];
                        }
                        delaypos /= drspos;
                        delaypos = (int) delaypos;
                        if (delaypos<0) delaypos=0;
                    } 
                    else 
                    {
                        if (slider6 != 0 && delaypos >= delaylen) delaypos = 0;
                    }
                }
                //freembuf(delaylen*2);
            }
            wetmix = pow(2,(slider2/6));
            drymix = pow(2,(slider3/6));
            wetmix2 = pow(2,(slider4/6));
            drymix2 = pow(2,(slider5/6));
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            int dpint = (int)delaypos*2;
            float os1=buffer[dpint+0];
            float os2=buffer[dpint+1];

            buffer[dpint+0]=Math.Min(Math.Max(spl0*drymix + os1*wetmix,-4),4);
            buffer[dpint+1]=Math.Min(Math.Max(spl1*drymix + os2*wetmix,-4),4);

            if ((delaypos += 1) >= delaylen)
            {
                delaypos = 0;
            }

            spl0=spl0*drymix2 + os1*wetmix2;
            spl1=spl1*drymix2 + os2*wetmix2;
        }
    }
}
