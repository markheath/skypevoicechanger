using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;

namespace SkypeVoiceChanger.Effects
{
    public class EffectStream : ISampleProvider
    {
        private readonly List<Effect> effects;
        private readonly ISampleProvider sourceProvider;
        private readonly object effectLock = new object();

        public EffectStream(ISampleProvider sourceProvider)
        {
            this.effects = new List<Effect>();
            this.sourceProvider = sourceProvider;
            foreach (var effect in effects)
            {
                InitialiseEffect(effect);
            }
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
                    effect.OnSample(ref sampleLeft, ref sampleRight);
                }

                // put them back
                buffer[offset++] = sampleLeft;
                if(WaveFormat.Channels == 2)
                {
                    buffer[offset++] = sampleRight;
                }
            }
        }


        public void UpdateEffectChain(Effect[] newEffects)
        {
            lock (effectLock)
            {
                foreach (var newEffect in newEffects.Except(effects))
                {
                    InitialiseEffect(newEffect);
                }
                effects.Clear();
                effects.AddRange(newEffects);
            }
        }

        private void InitialiseEffect(Effect effect)
        {
            effect.SampleRate = WaveFormat.SampleRate;
            effect.Init();
            effect.SliderChanged();
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
