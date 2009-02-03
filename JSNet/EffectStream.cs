using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Diagnostics;

namespace JSNet
{
    public class EffectStream : WaveStream
    {
        private EffectChain effects;
        public WaveStream source;
        private object effectLock = new object();
        private object sourceLock = new object();

        public EffectStream(EffectChain effects, WaveStream sourceStream)
        {
            this.effects = effects;
            this.source = sourceStream;
            foreach (Effect effect in effects)
            {
                InitialiseEffect(effect);
            }
        }

        public EffectStream(WaveStream sourceStream)
            : this(new EffectChain(), sourceStream)
        {        
        }

        public EffectStream(Effect effect, WaveStream sourceStream)
            : this(sourceStream)
        {
            AddEffect(effect);
        }

        public override WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        public override long Length
        {
            get { return source.Length; }
        }

        public override long Position
        {
            get { return source.Position; }
            set { lock (sourceLock) { source.Position = value; } }
        }        

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read;
            lock(sourceLock)
            {
                read = source.Read(buffer, offset, count);
            }
            if (WaveFormat.BitsPerSample == 16)
            {
                lock (effectLock)
                {
                    Process16Bit(buffer, offset, read);
                }
            }
            else if (WaveFormat.BitsPerSample == 32)
            {
                //Process32Bit(buffer, offset, read);
            }
            return read;
        }

        private void Process16Bit(byte[] buffer, int offset, int count)
        {
            int samples = count / 2;
            foreach (Effect effect in effects)
            {
                if (effect.Enabled)
                {
                    effect.Block(samples);
                }
            }

            for(int sample = 0; sample < samples; sample++)
            {
                // get the sample(s)
                int x = offset + sample * 2;
                short sample16Left = BitConverter.ToInt16(buffer, x);
                short sample16Right = sample16Left;
                if(WaveFormat.Channels == 2)
                {                    
                    sample16Right = BitConverter.ToInt16(buffer, x + 2);
                    sample++;
                }
               
                // run these samples through the effect
                float sample64Left = sample16Left / 32768.0f;
                float sample64Right = sample16Right / 32768.0f;
                foreach (Effect effect in effects)
                {
                    if (effect.Enabled)
                    {
                        effect.Sample(ref sample64Left, ref sample64Right);
                    }
                }

                //Debug.Assert(Math.Abs(sample64Left) <= 1.0);
                //Debug.Assert(Math.Abs(sample64Right) <= 1.0);

                sample16Left = (short)(sample64Left * 32768.0f);
                sample16Right = (short)(sample64Right * 32768.0f);

                // put them back
                buffer[x] = (byte)(sample16Left & 0xFF);
                buffer[x + 1] = (byte)((sample16Left >> 8) & 0xFF); 

                /*byte[] b = BitConverter.GetBytes(sample16Left);
                Debug.Assert(b[0] == buffer[x], "DOH");
                Debug.Assert(b[1] == buffer[x+1], "DUH");*/

                if(WaveFormat.Channels == 2)    
                {
                    buffer[x + 2] = (byte)(sample16Right & 0xFF);
                    buffer[x + 3] = (byte)((sample16Right >> 8) & 0xFF);
                }
            }
        }


        public bool MoveUp(Effect effect)
        {
            lock (effectLock)
            {
                return effects.MoveUp(effect);
            }
        }

        public bool MoveDown(Effect effect)
        {
            lock (effectLock)
            {
                return effects.MoveDown(effect);
            }
        }

        public void AddEffect(Effect effect)
        {
            InitialiseEffect(effect);
            lock (effectLock)
            {
                this.effects.Add(effect);
            }
        }

        private void InitialiseEffect(Effect effect)
        {
            effect.SampleRate = WaveFormat.SampleRate;
            effect.Init();
            effect.Slider();
        }

        public bool RemoveEffect(Effect effect)
        {
            lock (effectLock)
            {
                return this.effects.Remove(effect);
            }
        }
    }
}
