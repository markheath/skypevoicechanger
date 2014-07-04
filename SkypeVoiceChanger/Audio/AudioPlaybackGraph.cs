using System;
using System.Linq;
using NAudio.Wave;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger.Audio
{
    /// <summary>
    /// just for the playback part
    /// </summary>
    public class AudioPlaybackGraph : IDisposable
    {
        private readonly EffectChain effectChain;
        private WaveStream outStream;
        private IWavePlayer player;
        private EffectStream effectStream;

        public AudioPlaybackGraph(EffectChain effectChain)
        {
            this.effectChain = effectChain;
            effectChain.Modified += EffectChainOnModified;
        }

        private void EffectChainOnModified(object sender, EventArgs eventArgs)
        {
            if (effectStream != null)
                effectStream.UpdateEffectChain(effectChain.ToArray());
        }

        public bool FileLoaded
        {
            get
            {
                return outStream != null;
            }
        }

        public void LoadFile(string fileName)
        {
            if (outStream != null)
            {
                outStream.Dispose();
            }

            if (fileName.EndsWith(".mp3"))
            {
                outStream = new Mp3FileReader(fileName);
            }
            else if (fileName.EndsWith(".wav"))
            {
                outStream = new WaveFileReader(fileName);
            }
            else
            {
                throw new InvalidOperationException("Can't open this type of file");
            }

            if (outStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                // no longer needed for MP3, but this should let us support mu-law etc
                outStream = WaveFormatConversionStream.CreatePcmStream(outStream);
            }
        }

        public void Play()
        {
            effectStream = new EffectStream(outStream.ToSampleProvider());
            effectStream.UpdateEffectChain(effectChain.ToArray());
            CreatePlayer();
            player.Init(effectStream);
            player.Play();
        }

        private void CreatePlayer()
        {
            if (player == null)
            {
                var waveOut = new WaveOut();
                waveOut.DesiredLatency = 200; // 200ms
                waveOut.NumberOfBuffers = 2;
                waveOut.DeviceNumber = 0; // default device
                player = waveOut;               
            }
        }

        public void OnTimer()
        {
            if (player != null && player.PlaybackState == PlaybackState.Playing)
            {
                if (outStream.Position >= outStream.Length)
                {
                    player.Stop();
                }
            }
        }

        public void Pause()
        {
            if (player != null)
            {
                player.Pause();
            }
        }

        public void Dispose()
        {
            if (player != null)
            {
                player.Dispose();
                player = null;
            }
            if (outStream != null)
            {
                outStream.Dispose();
                outStream = null;
            }
        }

        public void Stop()
        {
            if (player != null)
            {
                player.Stop();
            }
        }

        public void Rewind()
        {
            if (outStream != null)
            {
                outStream.Position = 0;
            }
        }
    }
}
