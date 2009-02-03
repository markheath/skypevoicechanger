// This effect Copyright (C) 2004 and later Cockos Incorporated
// License: GPL - http://www.gnu.org/licenses/gpl.html
// ported to .NET by Mark Heath
using System;
using System.ComponentModel.Composition;

namespace JSNet
{
    [Export(typeof(Effect))]
    public class PitchDown : Effect
    {
        public PitchDown()
        {
            AddSlider(1, 0, 6, 1, "Octaves Down");
            AddSlider(0, 0, 11, 1, "Semitones Down");
            AddSlider(0, 0, 99, 1, "Cents Down");
            AddSlider(100, 4, 500, 10, "Chunk Size (ms)");
            AddSlider(1, 0.001f, 1, 0.1f, "Overlap Size");
            AddSlider(0, -120, 6, 1, "Wet Mix (dB)");
            AddSlider(-120, -120, 6, 1, "Dry Mix (dB)");
        }

        public override string Name
        {
            get { return "PitchDown"; }
        }

        int bufferSize;
        int bufferPosition;
        float scl;
        float rspos;
        float drymix;
        float wetmix;
        int rrilen;
        float[] buffer = new float[65534];
        float invbs;        

        public override void Slider()
        {            
            //lbufsize=bufsize; 
            int lbufsize = bufferSize;
            //bufsize=(srate*0.001*slider4)&65534; 
            bufferSize = (int)(SampleRate * 0.001 * slider4) & 65534;
            //lbufsize!=bufsize ? bufpos=0;
            if (lbufsize != bufferSize)
            {
                bufferPosition = 0;
            }

            //scl=2 ^ (-(slider1 + slider2/12 + slider3/1200)); 
            scl = pow(2, (-(slider1 + slider2 / 12.0f + slider3 / 1200.0f)));
            //rilen=(max(scl,0.5)*bufsize)|0; 
            int rilen = (int)(Math.Max(scl, 0.5) * bufferSize);
            //rrilen=((scl*bufsize)|0)*2; 
            rrilen = ((int)(scl * bufferSize)) * 2;
            //slider5=min(1,max(slider5,0)); 
            float overlap = Math.Min(1, Math.Max(slider5, 0));
            //rspos=slider5*(bufsize-rilen); 
            rspos = overlap * (bufferSize - rilen);
            //invbs=1/rspos;
            invbs = 1.0f / rspos;
            //drymix=2 ^ (slider7/6); 
            drymix = pow(2, slider7 / 6);
            //wetmix=2 ^ (slider6/6);
            wetmix = pow(2, slider6 / 6);
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            //ss0=spl0; ss1=spl1;
            float ss0 = spl0;
            float ss1 = spl1;

            //hbp=((bufpos * scl)|0)*2;
            int hbp = ((int)(bufferPosition * scl)) * 2;

            // pre read these before writing
            //s0r=rrilen[hbp];
            //s1r=rrilen[hbp+1];
            float s0r = buffer[rrilen + hbp];
            float s1r = buffer[rrilen + hbp + 1];

            //(bufpos*2)[0]=spl0;
            //(bufpos*2)[1]=spl1;
            buffer[bufferPosition * 2 + 0] = spl0;
            buffer[bufferPosition * 2 + 1] = spl1;

            //bufpos < rspos ? 
            if (bufferPosition < rspos)
            {
                // mix
                // sc=bufpos*invbs;
                // spl0=hbp[0]*sc + s0r*(1-sc);
                // spl1=hbp[1]*sc + s1r*(1-sc);
                float sc = bufferPosition * invbs;
                spl0 = buffer[hbp + 0] * sc + s0r * (1 - sc);
                spl1 = buffer[hbp + 1] * sc + s1r * (1 - sc);
            }
            else
            {
                // straight resample
                //     spl0=hbp[0]; 
                //     spl1=hbp[1];
                spl0 = buffer[hbp + 0];
                spl1 = buffer[hbp + 1];
            }

            //(bufpos+=1) >= bufsize ? bufpos=0;
            if (++bufferPosition >= bufferSize)
            {
                bufferPosition = 0;
            }

            //spl0 = spl0*wetmix + ss0*drymix;
            //spl1 = spl1*wetmix + ss1*drymix;
            spl0 = spl0 * wetmix + ss0 * drymix;
            spl1 = spl1 * wetmix + ss1 * drymix;
        }
    }
}
