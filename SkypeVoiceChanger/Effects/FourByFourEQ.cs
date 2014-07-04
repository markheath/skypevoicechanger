// original effect 4x4 from REAPER
// Ported to .NET by Mark Heath

namespace SkypeVoiceChanger.Effects
{
    //[Export(typeof(Effect))]
    public class FourByFourEQ : Effect
    {
        public FourByFourEQ()
        {
            AddSlider(0, 0, 100, 0.1f, "lo drive%");
            AddSlider(0, -12, 12, 0.1f, "lo gain");
            AddSlider(0, 0, 100, 0.1f, "mid drive%");
            AddSlider(0, -12, 12, 0.1f, "mid gain");
            AddSlider(0, 0, 100, 0.1f, "hi drive%");
            AddSlider(0, -12, 12, 0.1f, "hi gain");
            AddSlider(240, 60, 500, 1, "low-mid xover for multiband");
            AddSlider(2400, 510, 10000, 10, "mid-high xover for multiband");
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

        public override string Name
        {
            get { return "3 Band EQ with crossover"; }
        }

        float mixl;
        float mixm;
        float mixh;
        float drivel;
        float drivem;
        float driveh;
        float drivel1;
        float drivem1;
        float driveh1;
        float drivel2;
        float drivem2;
        float driveh2;
        float al;
        float ah;
        float mixl1;
        float mixm1;
        float mixh1;
        float mixl2;
        float mixm2;
        float mixh2;

        float gainl;
        float gainm;
        float gainh;
        float mixlg;
        float mixmg;
        float mixhg;
        float mixlg1;
        float mixmg1;
        float mixhg1;
        float mixlgd;
        float mixmgd;
        float mixhgd;

        public override void Slider()
        {
            mixl = slider1 / 100;
            mixm = slider3 / 100;
            mixh = slider5 / 100;
            drivel = mixl;
            drivem = mixm;
            driveh = mixh;
            drivel1 = 1 / (1 - (drivel / 2));
            drivem1 = 1 / (1 - (drivem / 2));
            driveh1 = 1 / (1 - (driveh / 2));
            drivel2 = drivel / 2;
            drivem2 = drivem / 2;
            driveh2 = driveh / 2;
            al = min(slider7, SampleRate) / SampleRate;
            ah = max(min(slider8, SampleRate) / SampleRate, al);

            mixl1 = 1 - mixl;
            mixm1 = 1 - mixm;
            mixh1 = 1 - mixh;
            mixl2 = mixl / 2;
            mixm2 = mixm / 2;
            mixh2 = mixh / 2;

            gainl = exp(slider2 * db2log);
            gainm = exp(slider4 * db2log);
            gainh = exp(slider6 * db2log);
            mixlg = mixl * gainl;
            mixmg = mixm * gainm;
            mixhg = mixh * gainh;

            mixlgd = mixl * gainl * drivel1;
            mixmgd = mixm * gainm * drivem1;
            mixhgd = mixh * gainh * driveh1;

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

            float wet0_l = mixlgd * low_l * (1 - abs(low_l * drivel2));
            float wet0_m = mixmgd * mid_l * (1 - abs(mid_l * drivem2));
            float wet0_h = mixhgd * high_l * (1 - abs(high_l * driveh2));
            float wet0 = (wet0_l + wet0_m + wet0_h);

            float dry0_l = low_l * mixlg1;
            float dry0_m = mid_l * mixmg1;
            float dry0_h = high_l * mixhg1;
            dry0 = (dry0_l + dry0_m + dry0_h);

            float wet1_l = mixlgd * low_r * (1 - abs(low_r * drivel2));
            float wet1_m = mixmgd * mid_r * (1 - abs(mid_r * drivem2));
            float wet1_h = mixhgd * high_r * (1 - abs(high_r * driveh2));
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
