// This effect Copyright (C) 2004 and later Cockos Incorporated
// License: GPL - http://www.gnu.org/licenses/gpl.html
// poreted to .NET by Mark Heath

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SkypeVoiceChanger.Effects
{
    [Export(typeof(Effect))]
    public class Chorus : Effect
    {
        public Chorus()
        {
            AddSlider(15,1,250,1,"chorus length (ms)");
            AddSlider(1,1,8,1,"number of voices");
            AddSlider(0.5f,0.1f,16,0.1f,"rate (hz)");
            AddSlider(0.7f,0,1,0.1f,"pitch fudge factor");
            AddSlider(-6,-100,12,1,"wet mix (dB)");
            AddSlider(-6,-100,12,1,"dry mix (dB)");
        }

        public override string Name
        {
            get { return "Chorus"; }
        }

        int bpos;

        public override void Init()
        {
            bpos = 0;
        }

        float numvoices;
        float choruslen;        
        float rateadj;
        float csize;
        int bufofs;
        float wetmix;
        float drymix;
        float[] buffer = new float[1000000];

        public override void Slider()
        {
            numvoices=min(16,max(slider2,1));
            choruslen=slider1*SampleRate*0.001f;

            for (int i = 0; i < numvoices; i++)
            {
                buffer[i] = (i + 1) / numvoices * PI;
            }
            
            bufofs=16384;

            csize=choruslen/numvoices * slider4;

            rateadj=slider3*2*PI/SampleRate;
            wetmix = pow(2,slider5/6);
            drymix = pow(2,slider6/6);
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            if(bpos >= choruslen) {
              bpos=0;
            }
            float os0=spl0;

            // calculate new sample based on numvoices
            spl0=spl0*drymix;
            float vol=wetmix/numvoices;
            
            for( int i = 0; i < numvoices; i++)
            {
               float tpos = bpos - (0.5f+0.49f*(sin( PI*(buffer[i] += rateadj))/(PI*buffer[i]))) * (i+1) * csize;

               if(tpos < 0) tpos += choruslen;
               if(tpos > choruslen) tpos -= choruslen;
               float frac=tpos-(int)tpos;
               float tpos2 = (tpos>=choruslen-1) ? 0:tpos+1;
              
               spl0 += (buffer[bufofs+(int)tpos]*(1-frac)+buffer[bufofs+(int)tpos2]*frac) * vol;
            }

            buffer[bufofs+bpos]=os0;
            bpos+=1;

            spl1=spl0;
        }
    }
}
