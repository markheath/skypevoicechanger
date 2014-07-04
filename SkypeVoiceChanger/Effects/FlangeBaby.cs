// Copyright 2007, Thomas Scott Stillwell
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
//
// Ported to .NET by Mark Heath, 28/11/2008

namespace SkypeVoiceChanger.Effects
{
    public class FlangeBaby : Effect
    {
        public FlangeBaby()
        {
            AddSlider(5, 1, 10, 0.01f, "Flange (Delay)");
            AddSlider(0.5f, 0, 1, 0.01f, "Depth");
            AddSlider(0, -1, 1, 0.01f, "Feedback");
            AddSlider(0, 0, 10, 0.01f, "Speed (Hz, 0=tempo)");
            AddSlider(0.5f, 0, 1, 0.01f, "Mix");
            AddSlider(0, 0, 5, 0.01f, "Channel Offset");
            AddSlider(0.25f, 0.0625f, 4, 0.0625f, "beatsync - fraction of whole note");
            Slider waveformSlider = AddSlider(0, 0, 1, 1, "LFO Waveform");

            waveformSlider.DiscreteValueText.Add("Triangle");
            waveformSlider.DiscreteValueText.Add("Sine");
        }

        public override string Name
        {
            get { return "FlangeBaby"; }
        }

        float[] buffer = new float[2048 + MAX_WG_DELAY * 2 + 2];
        const int MAX_WG_DELAY = 16384;
        //float i;
        int counter;
        int buffer0;
        int buffer1;
        float feedback;
        float delay;
        float tdelay0;
        float tdelay1;
        float rate;
        float mix;
        float beat;
        float triangle;
        float sinusoid;
        float lfo;
        float twopi;
        float offset;
        float sdelay0;
        float sdelay1;
        float depth;
        float tpos;
        float trate;
        int dir0;
        int dir1;

        public override void Init()
        {
            //i = 0;
            counter = 0;
            buffer0 = 2048;
            buffer1 = buffer0 + MAX_WG_DELAY;
            // buffer is alreay zero
            //memset(buffer0,0,MAX_WG_DELAY);
            //memset(buffer1,0,MAX_WG_DELAY);
            feedback = 0;
            delay = 5;
            tdelay0 = delay;
            tdelay1 = delay;
            rate = 0;
            mix = 0;
            beat = 0.25f;
            triangle = 0;
            sinusoid = 1;
            lfo = triangle;
            twopi = 2 * PI;
        }

        public override void Slider()
        {
            delay = slider1;
            offset = slider6;
            beat = 240 * slider7;
            tdelay0 = delay;
            tdelay1 = (delay + offset);
            sdelay0 = tdelay0 / 1000 * SampleRate;
            sdelay1 = tdelay1 / 1000 * SampleRate;
            feedback = slider3;
            depth = (delay - 0.1f) * slider2;
            mix = slider5;
            lfo = (slider8 == triangle ? triangle : sinusoid);
            tpos = 0;
        }

        public override void Block(int samplesperblock)
        {
            if (slider4 == 0)
            {
                rate = Tempo / beat;
            }
            else
            {
                rate = slider4;
            }

            if (lfo == triangle)
            {
                trate = 4 * depth / (SampleRate / rate);
            }
            else
            {
                trate = twopi / (SampleRate / rate);
            }
        }

        public override void Sample(ref float spl0, ref float spl1)
        {
            float back0 = counter - sdelay0;
            float back1 = counter - sdelay1;
            if (back0 < 0) back0 = MAX_WG_DELAY + back0;
            if (back1 < 0) back1 = MAX_WG_DELAY + back1;
            int index00 = (int)back0;
            int index01 = (int)back1;
            int index_10 = index00 - 1;
            int index_11 = index01 - 1;
            int index10 = index00 + 1;
            int index11 = index01 + 1;
            int index20 = index00 + 2;
            int index21 = index01 + 2;
            if (index_10 < 0) index_10 = MAX_WG_DELAY + 1;
            if (index_11 < 0) index_11 = MAX_WG_DELAY + 1;
            if (index10 >= MAX_WG_DELAY) index10 = 0;
            if (index11 >= MAX_WG_DELAY) index11 = 0;
            if (index20 >= MAX_WG_DELAY) index20 = 0;
            if (index21 >= MAX_WG_DELAY) index21 = 0;
            float y_10 = buffer[buffer0 + index_10];
            float y_11 = buffer[buffer1 + index_11];
            float y00 = buffer[buffer0 + index00];
            float y01 = buffer[buffer1 + index01];
            float y10 = buffer[buffer0 + index10];
            float y11 = buffer[buffer1 + index11];
            float y20 = buffer[buffer0 + index20];
            float y21 = buffer[buffer1 + index21];
            float x0 = back0 - index00;
            float x1 = back1 - index01;
            float c00 = y00;
            float c01 = y01;
            float c10 = 0.5f * (y10 - y_10);
            float c11 = 0.5f * (y11 - y_11);
            float c20 = y_10 - 2.5f * y00 + 2.0f * y10 - 0.5f * y20;
            float c21 = y_11 - 2.5f * y01 + 2.0f * y11 - 0.5f * y21;
            float c30 = 0.5f * (y20 - y_10) + 1.5f * (y00 - y10);
            float c31 = 0.5f * (y21 - y_11) + 1.5f * (y01 - y11);
            float output0 = ((c30 * x0 + c20) * x0 + c10) * x0 + c00;
            float output1 = ((c31 * x1 + c21) * x1 + c11) * x1 + c01;
            buffer[buffer0 + counter] = spl0 + output0 * feedback;
            buffer[buffer1 + counter] = spl1 + output1 * feedback;
            spl0 = spl0 * (1 - mix) + output0 * mix;
            spl1 = spl1 * (1 - mix) + output1 * mix;
            counter += 1;
            if (counter >= MAX_WG_DELAY) counter = 0;
            if (lfo == triangle)
            {
                if (dir0 != 0) tdelay0 += trate; else tdelay0 -= trate;
                if (dir1 != 0) tdelay1 += trate; else tdelay1 -= trate;
                if (tdelay0 >= delay + depth) dir0 = 0;
                if (tdelay1 >= delay + depth) dir1 = 0;
                if (tdelay0 <= delay - depth) dir0 = 1;
                if (tdelay1 <= delay - depth) dir1 = 1;
            }
            else
            {
                tdelay0 = delay + (delay - 0.1f) * sin(tpos);
                tdelay1 = delay + (delay - 0.1f) * sin(tpos + offset);
                tpos += trate;
                if (tpos > twopi) tpos = 0;
            }
            sdelay0 = tdelay0 / 1000 * SampleRate;
            sdelay1 = tdelay1 / 1000 * SampleRate;
        }
    }
}
