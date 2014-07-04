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
//Ported to .NET by Mark Heath

namespace SkypeVoiceChanger.Effects

{
    /// <summary>
    /// desc:compressor/limiter, similar to F670
    /// </summary>
    //[Export(typeof(Effect))]
    public class FairlyChildish : Effect
    {
        public FairlyChildish()
        {
            AddSlider(0, -60, 0, 0.1f, "Threshold (dB)");
            AddSlider(70, 0.1f, 100, 0.1f, "Bias");
            AddSlider(0, -30, 30, 0.1f, "Makeup gain");
            Slider agc = AddSlider(0, 0, 1, 1, "AGC");
            agc.DiscreteValueText.Add("Left/Right");
            agc.DiscreteValueText.Add("Lat/Vert");

            AddSlider(1, 1, 6, 1, "Time Constant");
            AddSlider(100, 1, 10000, 1, "Level detector RMS window");
            //AddSlider(1, 1, 50, 0.1, "Current comp. ratio");
            //AddSlider(0, -90, 0, 0.1, "Gain reduction");
        }

        public override string Name
        {
            get { return "FairlyChildish compressor/limiter"; }
        }

        float log2db;
        float db2log;
        //float i;
        float attime;
        float reltime;
        float rmstime;
        //float maxover;
        float ratio;
        float cratio;
        float rundb;
        float overdb;
        float atcoef;
        float relcoef;
        float rmscoef;
        float leftright;
        //float latvert;
        
        public override void Init()
        {
            log2db = 8.6858896380650365530225783783321f; // 20 / ln(10)
            db2log = 0.11512925464970228420089957273422f; // ln(10) / 20 
            //i = 0;
            attime = 0.0002f; //200us
            reltime = 0.300f; //300ms
            rmstime = 0.000050f; //50us
            //maxover = 0;
            ratio = 0;
            cratio = 0;
            rundb = 0;
            overdb = 0;
            atcoef = exp(-1 / (attime * SampleRate));
            relcoef = exp(-1 / (reltime * SampleRate));
            rmscoef = exp(-1 / (rmstime * SampleRate));
            leftright = 0;
            //latvert = 1;
        }

        float thresh;
        float threshv;
        float bias;
        float cthresh;
        float cthreshv;
        float makeup;
        float makeupv;
        float timeconstant;

        float agc;

        public override void Slider()
        {
            thresh = slider1;
            threshv = exp(thresh * db2log);
            ratio = 20;
            bias = 80 * slider2 / 100;
            cthresh = thresh - bias;
            cthreshv = exp(cthresh * db2log);
            makeup = slider3;
            makeupv = exp(makeup * db2log);
            agc = slider4;
            timeconstant = slider5;
            if (timeconstant == 1)
            {
                attime = 0.0002f;
                reltime = 0.300f;
            }
            if (timeconstant == 2)
            {
                attime = 0.0002f;
                reltime = 0.800f;
            }
            if (timeconstant == 3)
            {
                attime = 0.0004f;
                reltime = 2.000f;
            }
            if (timeconstant == 4)
            {
                attime = 0.0008f;
                reltime = 5.000f;
            }
            if (timeconstant == 5)
            {
                attime = 0.0002f;
                reltime = 10.000f;
            }
            if (timeconstant == 6)
            {
                attime = 0.0004f;
                reltime = 25.000f;
            }
            atcoef = exp(-1 / (attime * SampleRate));
            relcoef = exp(-1 / (reltime * SampleRate));

            rmstime = slider6 / 1000000;
            rmscoef = exp(-1 / (rmstime * SampleRate));
        }

        float aspl0;
        float aspl1;
        float runave;
        float dcoffset = 0; // never assigned to

        public override void Sample(ref float spl0, ref float spl1)
        {
            if (agc == leftright)
            {
                aspl0 = abs(spl0);
                aspl1 = abs(spl1);
            }
            else
            {
                aspl0 = abs(spl0 + spl1) / 2;
                aspl1 = abs(spl0 - spl1) / 2;
            }

            float maxspl = max(aspl0, aspl1);
            maxspl = maxspl * maxspl;

            runave = maxspl + rmscoef * (runave - maxspl);
            float det = sqrt(max(0, runave));

            overdb = 2.08136898f * log(det / threshv) * log2db;
            overdb = max(0, overdb);

            if (overdb > rundb)
            {
                rundb = overdb + atcoef * (rundb - overdb);
            }
            else
            {
                rundb = overdb + relcoef * (rundb - overdb);
            }
            overdb = max(rundb, 0);

            if (bias == 0)
            {
                cratio = ratio;
            }
            else
            {
                cratio = 1 + (ratio - 1) * sqrt((overdb + dcoffset) / (bias + dcoffset));
            }
            //slider7 = cratio;

            float gr = -overdb * (cratio - 1) / cratio;
            //slider8 = -gr;
            float grv = exp(gr * db2log);

            if (agc == leftright)
            {
                spl0 *= grv * makeupv;
                spl1 *= grv * makeupv;
            }
            else
            {
                float sav0 = (spl0 + spl1) * grv;
                float sav1 = (spl0 - spl1) * grv;
                spl0 = makeupv * (sav0 + sav1) * 0.5f;
                spl1 = makeupv * (sav0 - sav1) * 0.5f;
            }
        }
    }
}
