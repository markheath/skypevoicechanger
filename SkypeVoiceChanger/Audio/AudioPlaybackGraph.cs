using System;
using NAudio.Wave;
using SkypeVoiceChanger.Effects;

namespace SkypeVoiceChanger.Audio
{
    /// <summary>
    /// just for the playback part
    /// </summary>
    public class AudioPlaybackGraph : IDisposable
    {
        private WaveStream outStream;
        private IWavePlayer player;
        private EffectStream effectStream;
        private readonly EffectChain effectChain;

        public AudioPlaybackGraph(EffectChain effectChain)
        {
            this.effectChain = effectChain;
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
            effectStream = new EffectStream(effectChain, outStream.ToSampleProvider());
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

        public void AddEffect(Effect effect)
        {
            if (effectStream != null)
            {
                effectStream.AddEffect(effect);
            }
            else
            {
                effectChain.Add(effect);
            }

        }

        public void RemoveEffect(Effect effect)
        {
            if (effectStream != null)
            {
                effectStream.RemoveEffect(effect);
            }
            else
            {
                effectChain.Remove(effect);
            }

        }


        public bool MoveUp(Effect effect)
        {
            if (effectStream != null)
            {
                return effectStream.MoveUp(effect);
            }
            else
            {
                return effectChain.MoveUp(effect);
            }
        }

        public bool MoveDown(Effect effect)
        {
            if (effectStream != null)
            {
                return effectStream.MoveDown(effect);
            }
            else
            {
                return effectChain.MoveDown(effect);
            }
        }
    }
}
