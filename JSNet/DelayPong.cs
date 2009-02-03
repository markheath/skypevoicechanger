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
    /// <summary>
    /// ping-pong beatsync delay
    /// </summary>
    [Export(typeof(Effect))]
    public class DelayPong : Effect
    {
        public DelayPong()
        {
            AddSlider(0, 0, 13000, 1, "delay (ms) - 0 for beatsync");
            AddSlider(-5, -120, 6, 1, "feedback (dB)");
            AddSlider(0, -120, 6, 1, "mix in (dB)");
            AddSlider(-6, -120, 6, 1, "output wet (dB)");
            AddSlider(0, -120, 6, 1, "output dry (dB)");
            AddSlider(0, 0, 100, 1, "ping-pong width%");
            AddSlider(0.25f, 0.0625f, 4, 0.0625f, "beatsync - fraction of whole note");
        }

        public override string Name
        {
            get { return "DelayPong"; }
        }

        int delaypos;
        float pongloc;
        float delaylen;

        public override void Init()
        {
            delaypos = 0;
            pongloc = 0;
        }

        float odelay;
        float beat;
        float wetmix;
        float drymix;
        float wetmix2;
        float drymix2;
        float pongwidth;
        float pongpan;
        float[] buffer = new float[1000000];

        public override void Slider()
        {
            odelay = delaylen;
            beat = 240 * slider7;
            wetmix = pow(2, slider2 / 6);
            drymix = pow(2, slider3 / 6);
            wetmix2 = pow(2, slider4 / 6);
            drymix2 = pow(2, slider5 / 6);
            pongwidth = slider6 / 100;
            pongpan = (1 - pongwidth) / 2;
        }

        public override void Block(int samplesblock)
        {
            if (slider1 == 0)
            {
                delaylen = min((beat / Tempo) * SampleRate, 500000);
            }
            else
            {
                delaylen = min(slider1 * SampleRate / 1000, 500000);
            }
        }

        float sw;

        public override void Sample(ref float spl0, ref float spl1)
        {
            int dpint = delaypos * 2;
            float os1 = buffer[dpint + 0];
            float os2 = buffer[dpint + 1];

            buffer[dpint + 0] = min(max(spl0 * drymix + os1 * wetmix, -4), 4);
            buffer[dpint + 1] = min(max(spl1 * drymix + os2 * wetmix, -4), 4);

            //float switching = 0;

            if (abs(delaypos) < 400)
            {
                sw = (pongloc != 0) ? abs(delaypos) / 400 : ((400 - abs(delaypos)) / 400);
            }

            if ((delaypos += 1) >= delaylen)
            {
                delaypos = 0;
                pongloc = (pongloc * -1) + 1;
            }

            float os = (os1 + os2) / 2;
            float panloc = pongpan + pongwidth * sw;

            spl0 = spl0 * drymix2 + os * wetmix2 * (panloc);
            spl1 = spl1 * drymix2 + os * wetmix2 * (1 - panloc);
        }
    }
}
