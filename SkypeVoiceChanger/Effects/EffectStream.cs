using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Diagnostics;

namespace SkypeVoiceChanger.Effects
{
    public class EffectStream : ISampleProvider
    {
        private readonly EffectChain effects;
        private readonly ISampleProvider sourceProvider;
        private object effectLock = new object();

        public EffectStream(EffectChain effects, ISampleProvider sourceProvider)
        {
            this.effects = effects;
            this.sourceProvider = sourceProvider;
            foreach (var effect in effects)
            {
                InitialiseEffect(effect);
            }
        }

        public EffectStream(ISampleProvider sourceStream)
            : this(new EffectChain(), sourceStream)
        {
        }

        public EffectStream(Effect effect, ISampleProvider sourceStream)
            : this(sourceStream)
        {
            AddEffect(effect);
        }

        public WaveFormat WaveFormat
        {
            get { return sourceProvider.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = sourceProvider.Read(buffer, offset, count);

            lock (effectLock)
            {
                Process(buffer, offset, read);
            }
            return read;
        }

        private void Process(float[] buffer, int offset, int count)
        {
            int samples = count;
            foreach (var effect in effects.Where(e => e.Enabled))
            {
                effect.Block(samples);
            }

            
            for(int sample = 0; sample < samples; sample++)
            {
                float sampleLeft = buffer[offset];
                float sampleRight = sampleLeft;
                if(WaveFormat.Channels == 2)
                {
                    sampleRight = buffer[offset+1];
                    sample++;
                }
               
                // run these samples through the effect
                foreach (var effect in effects.Where(e => e.Enabled))
                {
                    effect.Sample(ref sampleLeft, ref sampleRight);
                }

                // put them back
                buffer[offset++] = sampleLeft;
                if(WaveFormat.Channels == 2)
                {
                    buffer[offset++] = sampleRight;
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
