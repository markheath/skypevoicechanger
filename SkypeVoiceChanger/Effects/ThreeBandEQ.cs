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
//ported to .NET by Mark Heath

namespace SkypeVoiceChanger.Effects
{
    //[Export(typeof(Effect))]
    public class ThreeBandEQ : Effect
    {

        public ThreeBandEQ()
        {
            AddSlider(0, 0, 100, 0.1f, "lo drive%");
            AddSlider(0, -12, 12, 0.1f, "lo gain");
            AddSlider(0, 0, 100, 0.1f, "mid drive%");
            AddSlider(0, -12, 12, 0.1f, "mid gain");
            AddSlider(0, 0, 100, 0.1f, "hi drive%");
            AddSlider(0, -12, 12, 0.1f, "hi gain");
            AddSlider(240, 60, 680, 1, "low-mid freq");
            AddSlider(2400, 720, 12000, 10, "mid-high freq");
        }

        public override string Name
        {
            get { return "3 Band EQ"; }
        }

        float db2log;
        float pi;
        float halfpi;
        float halfpiscaled;


        public override void Init()
        {
            db2log = 0.11512925464970228420089957273422f; // ln(10) / 20 
            pi = 3.1415926535f;
            halfpi = pi / 2;
            halfpiscaled = halfpi * 1.41254f;
        }
        float mixl;
        float mixm;
        float mixh;
        float al;
        float ah;
        float mixl1;
        float mixm1;
        float mixh1;
        float gainl;
        float gainm;
        float gainh;
        float mixlg;
        float mixmg;
        float mixhg;
        float mixlg1;
        float mixmg1;
        float mixhg1;

        public override void Slider()
        {
            mixl = slider1 / 100;
            mixm = slider3 / 100;
            mixh = slider5 / 100;
            al = min(slider7, SampleRate) / SampleRate;
            ah = max(min(slider8, SampleRate) / SampleRate, al);
            mixl1 = 1 - mixl;
            mixm1 = 1 - mixm;
            mixh1 = 1 - mixh;
            gainl = exp(slider2 * db2log);
            gainm = exp(slider4 * db2log);
            gainh = exp(slider6 * db2log);
            mixlg = mixl * gainl;
            mixmg = mixm * gainm;
            mixhg = mixh * gainh;
            mixlg1 = mixl1 * gainl;
            mixmg1 = mixm1 * gainm;
            mixhg1 = mixh1 * gainh;
        }
        
        float lfl;
        float lfh;
        float rfh;
        float rfl;

        public override void Sample(ref float spl0, ref float spl1)
        {
            float dry0 = spl0;
            float dry1 = spl1;

            float lf1h = lfh;
            lfh = dry0 + lfh - ah * lf1h;
            float high_l = dry0 - lfh * ah;

            float lf1l = lfl;
            lfl = dry0 + lfl - al * lf1l;
            float low_l = lfl * al;

            float mid_l = dry0 - low_l - high_l;

            float rf1h = rfh;
            rfh = dry1 + rfh - ah * rf1h;
            float high_r = dry1 - rfh * ah;

            float rf1l = rfl;
            rfl = dry1 + rfl - al * rf1l;
            float low_r = rfl * al;

            float mid_r = dry1 - low_r - high_r;

            float wet0_l = mixlg * sin(low_l * halfpiscaled);
            float wet0_m = mixmg * sin(mid_l * halfpiscaled);
            float wet0_h = mixhg * sin(high_l * halfpiscaled);
            float wet0 = (wet0_l + wet0_m + wet0_h);

            float dry0_l = low_l * mixlg1;
            float dry0_m = mid_l * mixmg1;
            float dry0_h = high_l * mixhg1;
            dry0 = (dry0_l + dry0_m + dry0_h);

            float wet1_l = mixlg * sin(low_r * halfpiscaled);
            float wet1_m = mixmg * sin(mid_r * halfpiscaled);
            float wet1_h = mixhg * sin(high_r * halfpiscaled);
            float wet1 = (wet1_l + wet1_m + wet1_h);

            float dry1_l = low_r * mixlg1;
            float dry1_m = mid_r * mixmg1;
            float dry1_h = high_r * mixhg1;
            dry1 = (dry1_l + dry1_m + dry1_h);

            spl0 = dry0 + wet0;
            spl1 = dry1 + wet1;
        }
    }
}
