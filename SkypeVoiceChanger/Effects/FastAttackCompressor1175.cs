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
//adapted to .NET by Mark Heath

using System;
using System.ComponentModel.Composition;

namespace SkypeVoiceChanger.Effects
{
    //[Export(typeof(Effect))]
    public class FastAttackCompressor1175 : Effect
    {


        public FastAttackCompressor1175()
        {
            AddSlider(0, -60, 0, 0.1f, "Threshold (dB)");
            Slider ratioSlider = AddSlider(1, 0, 3, 1, "Ratio"); // 
            AddSlider(0, -20, 20, 0.1f, "Gain");
            AddSlider(20, 20, 2000, 10, "Attack time (usec.)");
            AddSlider(250, 20, 1000, 1, "Release time (msec.)");
            AddSlider(100, 0, 100, 0.1f, "Mix%");
            ratioSlider.DiscreteValueText.Add("4");
            ratioSlider.DiscreteValueText.Add("8");
            ratioSlider.DiscreteValueText.Add("12");
            ratioSlider.DiscreteValueText.Add("20");
            ratioSlider.DiscreteValueText.Add("All");
        }

        public override string Name
        {
            get { return "Fast Attack Compressor 1175"; }
        }

        float log2db;
        float db2log;
        float attime;
        float reltime;
        float ratio;
        float cratio;
        float rundb;
        float overdb;
        float ratatcoef;
        float ratrelcoef;
        float atcoef;
        float relcoef;
        float mix;
        float gr_meter_decay;

        public override void Init()
        {
            log2db = 8.6858896380650365530225783783321f; // 20 / ln(10)
            db2log = 0.11512925464970228420089957273422f; // ln(10) / 20 
            attime = 0.010f;
            reltime = 0.100f;
            ratio = 0;
            cratio = 0;
            rundb = 0;
            overdb = 0;
            ratatcoef = exp(-1 / (0.00001f * SampleRate));
            ratrelcoef = exp(-1 / (0.5f * SampleRate));
            atcoef = exp(-1 / (attime * SampleRate));
            relcoef = exp(-1 / (reltime * SampleRate));
            mix = 1;
            gr_meter = 1;
            gr_meter_decay = exp(1 / (1 * SampleRate));
        }

        float thresh;
        float threshv;
        float allin;
        float cthresh;
        float cthreshv;
        float softknee = 0; // never assigned to
        float makeup;
        float makeupv;
        float autogain = 0; // never assigned to

        public override void Slider()
        {
            thresh = slider1;
            threshv = exp(thresh * db2log);
            ratio = (slider2 == 0 ? 4 : (slider2 == 1 ? 8 : (slider2 == 2 ? 12 : (slider2 == 3 ? 20 : 20))));
            if (slider2 == 4) { allin = 1; cratio = 20; } else { allin = 0; cratio = ratio; }
            cthresh = (softknee != 0) ? (thresh - 3) : thresh;
            cthreshv = exp(cthresh * db2log);
            makeup = slider3;
            makeupv = exp((makeup + autogain) * db2log);
            attime = slider4 / 1000000;
            reltime = slider5 / 1000;
            atcoef = exp(-1 / (attime * SampleRate));
            relcoef = exp(-1 / (reltime * SampleRate));
            mix = slider6 / 100;
        }

        float rmscoef = 0; // never assigned to
        float averatio;
        float runratio;
        float maxover;
        float gr_meter;
        float runmax;
        float runave;

        public override void Sample(ref float spl0, ref float spl1)
        {
            float ospl0 = spl0;
            float ospl1 = spl1;
            float aspl0 = Math.Abs(spl0);
            float aspl1 = Math.Abs(spl1);
            float maxspl = Math.Max(aspl0, aspl1);
            maxspl = maxspl * maxspl;
            runave = maxspl + rmscoef * (runave - maxspl);
            float det = sqrt(max(0,runave));

            overdb = 2.08136898f * log(det/cthreshv) * log2db;
            overdb = Math.Max(0,overdb);

            if(overdb - rundb > 5) averatio = 4;

            if(overdb > rundb) 
            {
                rundb = overdb + atcoef * (rundb - overdb);
                runratio = averatio + ratatcoef * (runratio - averatio);
            } 
            else 
            {
                rundb = overdb + relcoef * (rundb - overdb);
                runratio = averatio + ratrelcoef * (runratio - averatio);
            }
            overdb = rundb;
            averatio = runratio;

            if(allin != 0) 
            {
                cratio = 12 + averatio;
            } 
            else 
            {
                cratio = ratio;
            }

            float gr = -overdb * (cratio-1)/cratio;
            float grv = exp(gr * db2log);

            runmax = maxover + relcoef * (runmax - maxover);  // highest peak for setting att/rel decays in reltime
            maxover = runmax;

            if(grv < gr_meter) gr_meter=grv; else  { gr_meter*=gr_meter_decay; if(gr_meter>1)gr_meter=1; };

            spl0 *= grv * makeupv * mix;
            spl1 *= grv * makeupv * mix;  

            spl0 += ospl0 * (1-mix);
            spl1 += ospl1 * (1-mix);
        }

    }
}
