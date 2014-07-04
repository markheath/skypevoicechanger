using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger.Audio
{
    class AudioPipeline : IAudioProcessor
    {
        private readonly SkypeBufferProvider bufferStream;
        private readonly IWaveProvider outputProvider;
        private readonly MixingSampleProvider mixer;

        public AudioPipeline(EffectChain effects)
        {
            // Audio pipeline:
            // get the audio from Skype
            this.bufferStream = new SkypeBufferProvider(16000);
            // convert to 32 bit floating point
            var bufferStream32 = new Pcm16BitToSampleProvider(bufferStream);
            // pass through the effects
            var effectStream = new EffectStream(effects, bufferStream32);
            // now mix in any sound effects
            this.mixer = new MixingSampleProvider(effectStream.WaveFormat);
            this.mixer.AddMixerInput(effectStream);

            // and convert back to 16 bit ready to be given back to skype
            this.outputProvider = new SampleToWaveProvider16(mixer);
        }

        public void ProcessOutgoing(byte[] buffer, int count)
        {
            // give the input audio to the beginning of our audio graph
            bufferStream.SetLatestInBuffer(buffer);
            // process it out through the effects
            int read = outputProvider.Read(buffer, 0, count);
        }

        public void ProcessIncoming(byte[] buffer, int count)
        {
        }

        public void QueueForPlayback(string path)
        {
            WaveStream reader = new WaveFileReader(path);
            if (reader.WaveFormat.SampleRate != mixer.WaveFormat.SampleRate)
            {
                reader = new WaveFormatConversionStream(new WaveFormat(mixer.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels), reader);
            }

            var sampleReader = new Pcm16BitToSampleProvider(reader);
            /*ISampleProvider reader = new AudioFileReader(path);
            if (reader.WaveFormat.Channels > 1)
            {
                reader = new MultiplexingSampleProvider(new ISampleProvider[] { reader }, 1);
            }*/
            this.mixer.AddMixerInput(sampleReader);
        }
    }
}
