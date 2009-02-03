// This effect Copyright (C) 2004 and later Cockos Incorporated
// License: GPL - http://www.gnu.org/licenses/gpl.html
// ported to .NET by Mark Heath
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace JSNet
{
    [Export(typeof(Effect))]
    public class Volume : Effect
    {
        public Volume()
        {
            AddSlider(0,-150,150,1,"adjustment (dB)");
            AddSlider(0,-150,150,1,"max volume (dB)");
        }

        public override string Name
        {
            get { return "Volume adjustment"; }
        }
        
        float adj1;
        float adj2;
        float adj1_s;
        float dadj;
        float doseek;

        public override void Slider()
        {
            adj1=pow(2 , (slider1/6)); 
            adj2=pow(2 , (slider2/6));
            doseek=1;
        }

        public override void Block(int samplesblock)
        {
            if (doseek != 0)
            {
                dadj = (adj1 - adj1_s) / samplesblock;
                doseek = 0;
            }
            else
            {
                dadj = 0;
                adj1_s = adj1;
            }
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            spl0 = min(max(spl0 * adj1_s, -adj2), adj2);
            spl1 = min(max(spl1 * adj1_s, -adj2), adj2);
            adj1_s += dadj;
        }
    }
}
