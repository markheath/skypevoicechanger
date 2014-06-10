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

using System.ComponentModel.Composition;

namespace SkypeVoiceChanger.Effects
{
    /// <summary>
    /// peak-eating limiter
    /// </summary>
    [Export(typeof(Effect))]
    public class EventHorizon : Effect
    {
        public EventHorizon()
        {
            AddSlider(0.0f, -30.0f, 0.0f, 0.1f, "Threshold");
            AddSlider(-0.1f, -20.0f, 0.0f, 0.1f, "Ceiling");
            AddSlider(2.0f, 0, 6.0f, 0.01f, "Soft clip (dB)");
            AddSlider(10, 3, 20, 0.1f, "Soft clip ratio");
        }

        float log2db;
        float db2log;

        public override void Init()
        {
            // pi = 3.1415926535;
            log2db = 8.6858896380650365530225783783321f; // 20 / ln(10)
            db2log = 0.11512925464970228420089957273422f; // ln(10) / 20 
        }

        float thresh;
        float threshdb;
        float ceiling;
        float ceildb;
        float makeup;
        float makeupdb;
        float sc;
        float scv;
        float sccomp;
        float peakdb;
        float peaklvl;
        float scratio;
        float scmult;

        public override string Name
        {
            get { return "EventHorizon peak eating limiter"; }
        }

        public override void Slider()
        {
            thresh = exp(slider1 * db2log);
            threshdb = slider1;
            ceiling = exp(slider2 * db2log);
            ceildb = slider2;
            makeup = exp((ceildb - threshdb) * db2log);
            makeupdb = ceildb - threshdb;
            sc = -slider3;
            scv = exp(sc * db2log);
            sccomp = exp(-sc * db2log);
            peakdb = ceildb + 25;
            peaklvl = exp(peakdb * db2log);
            scratio = slider4;
            scmult = abs((ceildb - sc) / (peakdb - sc));
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            float peak = max(abs(spl0), abs(spl1));
            spl0 = spl0 * makeup;
            spl1 = spl1 * makeup;
            float sign0 = sign(spl0);
            float sign1 = sign(spl1);
            float abs0 = abs(spl0);
            float abs1 = abs(spl1);
            float overdb0 = 2.08136898f * log(abs0) * log2db - ceildb;
            float overdb1 = 2.08136898f * log(abs1) * log2db - ceildb;
            if (abs0 > scv)
            {
                spl0 = sign0 * (scv + exp(overdb0 * scmult) * db2log);
            }
            if (abs1 > scv)
            {
                spl1 = sign1 * (scv + exp(overdb1 * scmult) * db2log);
            }

            spl0 = min(ceiling, abs(spl0)) * sign(spl0);
            spl1 = min(ceiling, abs(spl1)) * sign(spl1);
        }
    }
}
