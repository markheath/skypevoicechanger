// Copyright 2006, Thomas Scott Stillwell
// All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted 
//provided that the following conditions are met:
//
//Redistributions of source code must retain the above copyright notice, this list of conditions 
//and the following disclaimer. 
//
//Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
//and the following disclaimer in the documentation and/or other materials provided with the distribution. 
//
//The name of Thomas Scott Stillwell may not be used to endorse or 
//promote products derived from this software without specific prior written permission. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
//IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
//BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
//PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF 
//THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//Ported to .NET by Mark Heath, http://mark-dot-net.blogspot.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace JSNet
{
    /// <summary>
    /// nonlinear waveshaper
    /// </summary>
    [Export(typeof(Effect))]
    public class BadBussMojo : Effect
    {
        public BadBussMojo()
        {
            AddSlider(0, -60, 0, 0.01f, "Pos. Thresh (dB)");
            AddSlider(0, -60, 0, 0.01f, "Neg. Thresh (dB)");
            AddSlider(1, 1, 2, 0.001f, "Pos. Nonlinearity");
            AddSlider(1, 1, 2, 0.001f, "Neg. Nonlinearity");
            AddSlider(0, 0, 6, 0.01f, "Pos. Knee (dB)");
            AddSlider(0, 0, 6, 0.01f, "Neg. Knee (dB)");
            AddSlider(0, 0, 100, 0.1f, "Mod A");
            AddSlider(0, 0, 100, 0.1f, "Mod B");
        }

        float log2db;
        float db2log;
        float pi;
        float halfpi;

        public override string Name
        {
            get { return "BadBussMojo"; }
        }

        public override void Init()
        {
            log2db = 8.6858896380650365530225783783321f; // 20 / ln(10)
            db2log = 0.11512925464970228420089957273422f; // ln(10) / 20 
            pi = 3.1415926535f;
            halfpi = pi / 2;
        }

        float pt;
        float nt;
        float pl;
        float nl;
        float mixa;
        float mixb;
        float drivea;
        float mixa1;
        float drivea1;
        float drivea2;
        float mixb1;
        float pts;
        float nts;
        float ptt;
        float ntt;
        float ptsv;
        float ntsv;
        float drive = 0; // never assigned to

        public override void Slider()
        {
            pt = slider1;
            nt = slider2;
            pl = slider3 - 1;
            nl = slider4 - 1;
            mixa = slider7 / 100;
            mixb = slider8 / 100;
            drivea = 1;
            mixa1 = 1 - mixa;
            drivea1 = 1 / (1 - (drivea / 2));
            drivea2 = drive / 2;
            mixb1 = 1 - mixb;
            pts = slider5;
            nts = slider6;
            ptt = pt - pts;
            ntt = nt - nts;

            ptsv = exp(ptt * db2log);
            ntsv = -exp(ntt * db2log);
        }

        float wet0;
        float wet1;
        float diff;
        float mult;

        public override void Sample(ref float spl0, ref float spl1)
        {
            if (mixa > 0)
            {
                wet0 = drivea1 * spl0 * (1 - Math.Abs(spl0 * drivea2));
                wet1 = drivea1 * spl1 * (1 - Math.Abs(spl1 * drivea2));
                spl0 = mixa1 * spl0 + (mixa) * wet0;
                spl1 = mixa1 * spl1 + (mixa) * wet1;
            }

            if (mixb > 0)
            {
                wet0 = sin(spl0 * halfpi);
                wet1 = sin(spl1 * halfpi);
                spl0 = mixb1 * spl0 + (mixb) * wet0;
                spl1 = mixb1 * spl1 + (mixb) * wet1;
            }

            float db0 = log(abs(spl0)) * log2db;
            float db1 = log(abs(spl1)) * log2db;

            if (spl0 > ptsv)
            {
                diff = max(min((db0 - ptt), 0), pts);
                if (pts == 0) mult = 0; else mult = diff / pts;
                spl0 = ptsv + ((spl0 - ptsv) / (1 + (pl * mult)));
            }
            if (spl0 < ntsv)
            {
                diff = max(min((db0 - ntt), 0), nts);
                if(nts == 0) mult = 0; else mult = diff / nts;
                spl0 = ntsv + ((spl0 - ntsv) / (1 + (nl * mult)));
            }
            if (spl1 > ptsv)
            {
                diff = max(min((db1 - ptt), 0), pts);
                if(pts == 0) mult = 0; else mult = diff / pts;
                spl1 = ptsv + ((spl1 - ptsv) / (1 + (pl * mult)));
            }
            if (spl1 < ntsv)
            {
                diff = max(min((db1 - ntt), 0), nts);
                if(nts == 0) mult = 0; else mult = diff / nts;
                spl1 = ntsv + ((spl1 - ntsv) / (1 + (nl * mult)));
            }
        }
    }
}
